using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerListObject : MonoBehaviour
{
    public TextMeshProUGUI playerName;

    private void Start() {
        this.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }
    public void Setup(string playerN){
        playerName.text = playerN;
    }
}
