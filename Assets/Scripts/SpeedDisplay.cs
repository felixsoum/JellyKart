using UnityEngine;
using UnityEngine.UI;

public class SpeedDisplay : MonoBehaviour
{
    public new Rigidbody rigidbody;
    Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }
	
	void Update()
    {
        int speed = (int)rigidbody.velocity.magnitude;
        text.text = speed.ToString();	
	}
}
