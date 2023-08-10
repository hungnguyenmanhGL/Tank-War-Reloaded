using UnityEngine;

namespace HAVIGAME.Services.IAP {

    [DefineSymbols(IAPManager.DEFINE_SYMBOL)]
    [SettingMenu(typeof(IAPSettings), "Services/In-App Puchasing", "", null, 101, "Icons/icon_iap")]
    [CreateAssetMenu(fileName = "IAPSettings", menuName = "HAVIGAME/Settings/Services/In-App Purchasing")]
    public class IAPSettings : Database<IAPSettings> {

        [SerializeField] private bool purchaseOrderValidate
            ;
        [SerializeField] private bool gameServiceEnabled = false;
        [SerializeField] private IAPProduct[] products;

        public bool PurchaseOrderValidate => purchaseOrderValidate;
        public bool GameServiceEnabled => gameServiceEnabled;
        public IAPProduct[] Products => products;


        public IAPProduct GetProduct(string productId) {
            foreach (IAPProduct product in products) {
                if (product.Id.Equals(productId)) {
                    return product;
                }
            }

            return null;
        }
    }
}
