using UnityEngine;

namespace HAVIGAME.Services.Advertisings {
    [System.Serializable]
    public abstract class AdId {

        public bool IsNullOrEmpty => string.IsNullOrEmpty(Value);

        public virtual string Value => string.Empty;

        public virtual string MoveNext() {
            return Value;
        }

        public virtual string Reset() {
            return Value;
        }
    }

    [System.Serializable]
    [CategoryMenu("Single ID")]
    public class SingleAdId : AdId {
        public static readonly SingleAdId Empty = new SingleAdId();

        [SerializeField] private StringPropertyReadonly id = StringPropertyReadonly.Create();

        public override string Value => id.Get();
    }

    [System.Serializable]
    [CategoryMenu("Sequence ID", 1)]
    public class SequenceAdId : AdId {
        [SerializeField] private StringPropertyReadonly[] sequenceIds;

        private int index = 0;

        public override string Value => sequenceIds[index].Get();

        public override string MoveNext() {
            index++;
            if (index >= sequenceIds.Length) {
                index = 0;
            }

            return Value;
        }

        public override string Reset() {
            index = 0;
            return Value;
        }
    }
}
