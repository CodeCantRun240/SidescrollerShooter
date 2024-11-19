using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MessageBox : MonoBehaviour
{
    public GameObject messagePanel;  // Reference to the Panel (the background of the message box)
    public TextMeshProUGUI messageText;  // Reference to the Text component inside the message box
    public float messageDuration = 3f;  // Duration before the message box disappears

    private Coroutine autoHideCoroutine;  // To store the coroutine for auto-hide

    private void Start()
    {
        // Ensure the message panel is hidden at the start
        messagePanel.SetActive(false);
    }

    // This method can be called from any script to display the message box with the specified text
    public void ShowMessage(string text)
    {
        // Stop any previous auto-hide coroutine to prevent conflicts
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
        }

        messageText.text = text;  // Set the text in the message box
        messagePanel.SetActive(true);  // Show the message box
        
        // Start the auto-hide coroutine
        autoHideCoroutine = StartCoroutine(AutoHideMessage());
    }

    // Coroutine that hides the message box after 3 seconds
    private IEnumerator AutoHideMessage()
    {
        yield return new WaitForSeconds(messageDuration);  // Wait for the specified duration
        HideMessage();  // Hide the message after the delay
    }

    // This method will be called when the panel or button is clicked to hide the message box
    public void HideMessage()
    {
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);  // Stop any running auto-hide coroutine if the user hides it manually
            autoHideCoroutine = null;
        }
        messagePanel.SetActive(false);  // Hide the message box
    }
}
