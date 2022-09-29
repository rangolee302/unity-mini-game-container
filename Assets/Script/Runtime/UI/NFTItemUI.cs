using UnityEngine;
using System;
using TMPro;

namespace SuperUltra.Container
{

    public class NFTItem
    {
        public int id;
        public string name; 
        public string description;
        public Texture2D texture2D;
    }

    public class NFTItemUI : MonoBehaviour
    {
        // TODO : item structure
        [SerializeField] TMP_Text _itemName;

        public void Initialize(NFTItem item)
        {
            _itemName.text = item.name;
        }

    }

}