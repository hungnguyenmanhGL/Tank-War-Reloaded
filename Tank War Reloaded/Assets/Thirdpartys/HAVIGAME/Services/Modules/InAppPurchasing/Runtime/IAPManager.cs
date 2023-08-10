using System;
using UnityEngine;
using System.Collections.Generic;

#if IAP
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.Purchasing.Extension;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
#endif


namespace HAVIGAME.Services.IAP {
    public static class IAPManager {

        public const string DEFINE_SYMBOL = "IAP";

        public static readonly InitializeEvent initializeEvent = new InitializeEvent();

#if IAP
        private static StoreListener storeListener;
        private static ProductTransaction productTransaction;
#endif

        public static bool IsInitialized => initializeEvent.IsInitialized;

        public static void Initialize() {
            IAPSettings settings = IAPSettings.Instance;

#if IAP
            if (initializeEvent.IsRunning) {
                Log.Warning("[IAPManager] Cancel initialize! In-App Purchasing initialized with result {0}", initializeEvent.IsInitialized);
                return;
            }

            if (settings.GameServiceEnabled) {
                InitializeGamingServices(OnInitializeGamingServicesCompleted, OnInitializeGamingServicesFailed);
            }
            else {
                InitializeInAppPurchase();
            }
#endif
        }

        public static bool IsOwned(string productId) {
#if IAP
            if (!IsInitialized) {
                Log.Warning("[IAPManager] In-App Purchasing is not initialized.");
                return false;
            }
            return storeListener.IsOwned(productId);
#else
            return false;
#endif
        }

        public static bool IsSubscribed(string productId) {
#if IAP
            if (!IsInitialized) {
                Log.Warning("[IAPManager] In-App Purchasing is not initialized.");
                return false;
            }
            return storeListener.IsOwned(productId);
#else
            return false;
#endif
        }

        public static bool Purchase(string productId, Action onCompleted = null, Action<string> onFailed = null) {
#if IAP
            if (!IsInitialized) {
                Log.Warning("[IAPManager] Purchase failed! In-App Purchasing is not initialized.");
                return false;
            }

            Product product = storeListener.GetProduct(productId);

            if (product == null) {
                return false;
            }

            productTransaction.StartTransaction(productId, onCompleted, onFailed);
            return true;
#else
            Log.Info("[IAPManager] Purchase failed! In-App Purchasing is not enabled.");
            onFailed?.Invoke("In-App Purchasing is not enabled");
            return false;
#endif
        }

        public static void RestorePurchases(Action<string[]> onCompleted = null, Action<string> onFailed = null) {
#if IAP
            if (!IsInitialized) {
                Log.Warning("[IAPManager] Restore purchase failed! In-App Purchasing is not initialized.");
                onFailed?.Invoke("In-App Purchasing is not initialized.");
            }

            storeListener.RestorePurchases(onCompleted, onFailed);
#else
            Log.Info("[IAPManager] Restore purchase failed! In-App Purchasing is not enabled.");
            onFailed?.Invoke("In-App Purchasing is not enabled");
#endif
        }

        public static string GetLocalizedPriceString(string productId, string defaultPrice = "$0.99") {
#if IAP
            if (!IsInitialized) {
                Log.Warning("[IAPManager] In-App Purchasing is not initialized.");
                return defaultPrice;
            }

            Product product = storeListener.GetProduct(productId);
            if (product != null) {
                return product.metadata.localizedPriceString;
            }
            else {
                return defaultPrice;
            }
#else
            return defaultPrice;
#endif
        }

        public static decimal GetLocalizedPrice(string productId, decimal defaultPrice = 0.99m) {
#if IAP
            if (!IsInitialized) {
                Log.Warning("[IAPManager] In-App Purchasing is not initialized.");
                return defaultPrice;
            }

            Product product = storeListener.GetProduct(productId);
            if (product != null) {
                return product.metadata.localizedPrice;
            }
            else {
                return defaultPrice;
            }
#else
            return defaultPrice;
#endif
        }

        public static string GetIsoCurrencyCode(string productId, string defaultIsoCurrencyCode = "$") {
#if IAP
            if (!IsInitialized) {
                Log.Warning("[IAPManager] In-App Purchasing is not initialized.");
                return defaultIsoCurrencyCode;
            }

            Product product = storeListener.GetProduct(productId);
            if (product != null) {
                return product.metadata.isoCurrencyCode;
            }
            else {
                return defaultIsoCurrencyCode;
            }
#else
            return defaultIsoCurrencyCode;
#endif
        }

        public static DateTime GetSubscriptionPurchaseDate(string productId) {
#if IAP
            if (!IsInitialized) {
                Log.Warning("[IAPManager] In-App Purchasing is not initialized.");
                return default;
            }
            return storeListener.GetSubscriptionPurchaseDate(productId);
#else
            return default;
#endif
        }

#if IAP
        private static void InitializeGamingServices(Action onCompleted, Action<string> onFailed) {
            Log.Debug("[IAPManager] Initialize the gaming services...");
            try {
                var options = new InitializationOptions().SetEnvironmentName("production");

                UnityServices.InitializeAsync(options).ContinueWith(task => Executor.Instance.RunOnMainTheard(onCompleted));
            }
            catch (Exception exception) {
                Executor.Instance.RunOnMainTheard(() => onFailed?.Invoke(exception.Message));
            }
        }

        private static void OnInitializeGamingServicesCompleted() {
            Log.Debug("[IAPManager] Initialize the gaming services completed!");

            InitializeInAppPurchase();
        }

        private static void OnInitializeGamingServicesFailed(string error) {
            Log.Error("[IAPManager] Initialize the gaming services failed!");
            initializeEvent.Invoke(false);
        }

        private static void InitializeInAppPurchase() {
            Log.Debug("[IAPManager] Initialize the in-app purchasing...");

            IAPSettings settings = IAPSettings.Instance;

            ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (IAPProduct product in settings.Products) {
                builder.AddProduct(product.Id, Convert(product.Type));
            }

            storeListener = new StoreListener(settings.PurchaseOrderValidate);

            storeListener.onInitialized += OnInAppPurchaseInitializeCompleted;
            storeListener.onInitializeFailed += OnInAppPurchaseInitializeFailed;

            UnityPurchasing.Initialize(storeListener, builder);

            Database.Unload(settings);
        }

        private static void OnInAppPurchaseInitializeCompleted() {
            Log.Info("[IAPManager] Initialize the in-app purchasing completed!");
            productTransaction = new ProductTransaction(storeListener);

            initializeEvent.Invoke(true);
        }

        private static void OnInAppPurchaseInitializeFailed(string error) {
            Log.Debug("[IAPManager] Initialize the in-app purchasing failed, error = {0}", error);

            initializeEvent.Invoke(false);
        }

        private static ProductType Convert(IAPProductType productType) {
            return (ProductType)productType;
        }

        private class StoreListener : IDetailedStoreListener {
            private bool purchaseOrderValidate;
            private IStoreController controller;
            private IExtensionProvider extensions;

            public event IAPDelegate onInitialized;
            public event IAPErrorDelegate onInitializeFailed;
            public event IAPPurchaseDelegate onPurchased;
            public event IAPPurchaseErrorDelegate onPurchaseFailed;

            public StoreListener(bool purchaseOrderValidate) {
                this.purchaseOrderValidate = purchaseOrderValidate;
                this.controller = null;
                this.extensions = null;
            }

            public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
                this.controller = controller;
                this.extensions = extensions;

                onInitialized?.Invoke();
            }

            public void OnInitializeFailed(InitializationFailureReason error) {

            }

            public void OnInitializeFailed(InitializationFailureReason error, string message) {
                onInitializeFailed?.Invoke(message);
            }

            public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
                onPurchaseFailed?.Invoke(product.definition.id, failureReason.ToString());
            }

            public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription) {
                onPurchaseFailed?.Invoke(product.definition.id, Utility.Text.Format("{0} - {1}", failureDescription.reason, failureDescription.message));
            }

            public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent) {
                bool validPurchase = true;

                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer) {

                    if (purchaseOrderValidate) {
                        CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

                        try {
                            IPurchaseReceipt[] result = validator.Validate(purchaseEvent.purchasedProduct.receipt);

                            Log.Debug("Receipt is valid. Contents:");
                            foreach (IPurchaseReceipt productReceipt in result) {
                                Log.Debug("productID={0}, date={1}, transactionID={2}", productReceipt.productID, productReceipt.purchaseDate, productReceipt.transactionID);
                            }
                            validPurchase = true;
                        }
                        catch (IAPSecurityException) {
                            Log.Error("Invalid receipt!");
                            validPurchase = false;
                        }
                    }
                    else {
                        validPurchase = true;
                        Log.Debug("Receipt is valid. productID={0}", purchaseEvent.purchasedProduct.definition.id);
                    }
                }

                if (validPurchase) {
                    onPurchased?.Invoke(purchaseEvent.purchasedProduct.definition.id);
                }
                else {
                    onPurchaseFailed?.Invoke(purchaseEvent.purchasedProduct.definition.id, "Invalid receipt");
                }
                return PurchaseProcessingResult.Complete;
            }

            public void InitiatePurchase(string productId) {
                controller.InitiatePurchase(productId);
            }

            public void RestorePurchases(Action<string[]> onCompleted = null, Action<string> onFailed = null) {
                if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer) {
                    IAppleExtensions appleExtension = GetExtension<IAppleExtensions>();
                    appleExtension.RestoreTransactions((result, error) => {
                        if (result) {
                            List<string> restoredProducts = new List<string>();
                            foreach (Product product in GetProducts(ProductType.NonConsumable)) {
                                if (IsOwned(product.definition.id)) {
                                    restoredProducts.Add(product.definition.id);
                                }
                            }
                            onCompleted?.Invoke(restoredProducts.ToArray());
                        }
                        else {
                            onFailed?.Invoke(error);
                        }
                    });
                }
                else if (Application.platform == RuntimePlatform.Android) {
                    List<string> restoredProducts = new List<string>();
                    foreach (Product product in GetProducts(ProductType.NonConsumable)) {
                        if (IsOwned(product.definition.id)) {
                            restoredProducts.Add(product.definition.id);
                        }
                    }
                    onCompleted?.Invoke(restoredProducts.ToArray());
                }
                else {
                    onFailed?.Invoke("In-App Purchasing is no supported this platform.");
                }
            }

            public Product GetProduct(string productId) {
                return controller.products.WithID(productId);
            }

            public IEnumerable<Product> GetProducts(ProductType productType) {
                foreach (Product product in controller.products.all) {
                    if (product.definition.type == productType) {
                        yield return product;
                    }
                }
            }

            public bool IsOwned(string productId) {
                Product product = GetProduct(productId);

                bool hasReceipt = product.hasReceipt;

                return hasReceipt;
            }

            public bool IsSubscribed(string productId) {
                SubscriptionInfo subscriptionInfo = GetSubscriptionInfo(productId);

                if (subscriptionInfo != null) {
                    return subscriptionInfo.isSubscribed() == UnityEngine.Purchasing.Result.True;
                }

                return false;
            }

            public DateTime GetSubscriptionPurchaseDate(string productId) {
                SubscriptionInfo subscriptionInfo = GetSubscriptionInfo(productId);

                if (subscriptionInfo != null) {
                    return subscriptionInfo.getPurchaseDate();
                }

                return default;
            }

            public T GetExtension<T>() where T : IStoreExtension {
                return extensions.GetExtension<T>();
            }

            public UnityEngine.Purchasing.SubscriptionInfo GetSubscriptionInfo(string productId) {
                if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer) {
                    Log.Warning("Getting subscription info is only available on Android and iOS.");
                    return null;
                }

                Product product = GetProduct(productId);

                if (product.definition.type != ProductType.Subscription) {
                    Log.Debug("Couldn't get subscription info: this product is not a subscription product.");
                    return null;
                }

                if (!product.hasReceipt) {
                    Log.Debug("Couldn't get subscription info: this product doesn't have a valid receipt.");
                    return null;
                }

                if (Application.platform == RuntimePlatform.Android) {
                    SubscriptionManager subscriptionManager = new SubscriptionManager(product, null);
                    return subscriptionManager.getSubscriptionInfo();
                }
                else {
                    IAppleExtensions appleExtensions = GetExtension<IAppleExtensions>();
                    Dictionary<string, string> introPriceDictionary = appleExtensions.GetIntroductoryPriceDictionary();
                    string introJson = null;

                    if (introPriceDictionary != null && introPriceDictionary.ContainsKey(product.definition.storeSpecificId)) {
                        introJson = introPriceDictionary[product.definition.storeSpecificId];
                    }

                    SubscriptionManager subscriptionManager = new SubscriptionManager(product, introJson);
                    return subscriptionManager.getSubscriptionInfo();
                }
            }
        }

        private class ProductTransaction : IDisposable {
            private StoreListener storeListener;
            private bool isTransacting;
            private string productId;
            private Action onCompleted;
            private Action<string> onFailed;

            public bool IsTransacting => isTransacting;

            public ProductTransaction(StoreListener storeListener) {
                this.storeListener = storeListener;
                this.isTransacting = false;

                storeListener.onPurchased += OnPurchased;
                storeListener.onPurchaseFailed += OnPurchasFailed;
            }

            public void Dispose() {
                storeListener.onPurchased -= OnPurchased;
                storeListener.onPurchaseFailed -= OnPurchasFailed;
                isTransacting = false;
                storeListener = null;
            }

            public void StartTransaction(string productId, Action onCompleted, Action<string> onFailed) {
                if (string.IsNullOrEmpty(productId)) {
                    onFailed?.Invoke("Start transaction failed! Product id is null or empty");
                }
                else {
                    this.productId = productId;
                    this.onCompleted = onCompleted;
                    this.onFailed = onFailed;
                    isTransacting = true;
                    storeListener.InitiatePurchase(productId);
                }
            }

            private void OnPurchased(string productId) {
                if (string.IsNullOrEmpty(this.productId)) {
                    isTransacting = false;
                    onFailed?.Invoke("Purchase finished with error! Product id is null or empty");
                }
                else if (this.productId.Equals(productId)) {
                    isTransacting = false;
                    onCompleted?.Invoke();
                }
            }

            private void OnPurchasFailed(string productId, string error) {
                if (string.IsNullOrEmpty(this.productId)) {
                    isTransacting = false;
                    onFailed?.Invoke("Purchase finished with error! Product id is null or empty");
                }
                else if (this.productId.Equals(productId)) {
                    isTransacting = false;
                    onFailed?.Invoke(error);
                }
            }
        }
#endif


#if IAP
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterModule() {
            GameManager.RegisterModule<Initializer>();
        }

        public class Initializer : ModuleInitializer {

            public override int Order => SERVICE;
            public override InitializeEvent InitializeEvent => IAPManager.initializeEvent;

            public override void Initialize() {
                IAPManager.Initialize();
            }
        }
#endif
    }
}
