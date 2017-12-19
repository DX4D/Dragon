using UnityEngine;
using MalbersAnimations.Events;

namespace MalbersAnimations
{
    //Basic inventory veeeery Basic
    [System.Serializable]
    public class InventorySlot
    {
        public GameObject item;
        public InputRow input;
    }  

    public class MInventory : MonoBehaviour
    {
        public InventorySlot[] Inventory;

        public GameObjectEvent OnEquipItem;

        void Update()
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i].input.GetInput)
                {
                    EquipItem(i);
                    break;
                }
            }
        }

        public virtual void EquipItem(int Slot)
        {
            OnEquipItem.Invoke(Inventory[Slot].item);
        }
    }
}
