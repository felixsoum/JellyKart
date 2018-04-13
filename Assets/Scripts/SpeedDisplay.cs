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
        text.text = rigidbody.velocity.magnitude.ToString();	
	}
}
