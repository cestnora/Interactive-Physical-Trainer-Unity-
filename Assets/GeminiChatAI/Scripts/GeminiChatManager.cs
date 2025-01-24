using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;
using TMPro;
//using SpeechLib;

public class GeminiChatManager : MonoBehaviour
{
    public TMP_InputField playerInputField;
    public string apiKey = "yourAPIKey";
    private string apiUrl = "yourAPIURL";
    public ScrollRect scrollRect;
    public RectTransform content;
    public GameObject newTextPrefab;
    //SpVoice voice = new SpVoice();
    void Start()
    {
        StartChat();
    }

    public void StartChat()
    {
        string text = "Hi! Welcome to your virtual physical trainer! How can I assist you today? You can ask me for workouts, warm-up tips, recovery advice, or just motivation!";
        AddToChat($"Trainer: {text}");
        //voice.Speak(text, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
    }

    public void OnSendButtonClick()
    {
        string playerMessage = playerInputField.text;
        if (string.IsNullOrEmpty(playerMessage)) return;
        string message = AddLineBreaks(playerMessage, 38);
        AddPlayerChat($"You: {message}");
        HandleUserMessage(message);
        playerInputField.text = "";
    }

    private string AddLineBreaks(string text, int maxCharactersPerLine)
    {
        string processedText = "";
        int currentLineLength = 0;

        foreach (char c in text)
        {
            processedText += c;
            currentLineLength++;

            if (currentLineLength >= maxCharactersPerLine && c == ' ')
            {
                processedText += "\n";
                currentLineLength = 0;
            }
        }

        return processedText;
    }

    private void HandleUserMessage(string message)
    {
        // Basic intent recognition
        if (message.ToLower().Contains("workout"))
        {
            SuggestWorkout();
        }
        else if (message.ToLower().Contains("warm-up"))
        {
            SuggestWarmUp();
        }
        else if (message.ToLower().Contains("recovery"))
        {
            SuggestRecoveryTips();
        }
        else if (message.ToLower().Contains("motivation"))
        {
            ProvideMotivation();
        }
        else
        {
            StartCoroutine(SendToChatGPT(message));
        }
    }

    private void SuggestWorkout()
    {
        string text = "Here's a quick full-body workout for you: 10 Push-ups + 15 Squats + 20 Mountain Climbers + 30-Second Plank. Repeat 3 times for a great workout!";
        AddToChat($"Trainer: {text}");
        //voice.Speak(text, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
    }

    private void SuggestWarmUp()
    {
        string text = "A quick warm-up to get your body ready: " +
                  "2 minutes of light jogging in place. 10 Arm Circles (each direction). 10 Leg Swings (each leg). 10 Bodyweight Squats";
        AddToChat($"Trainer: {text}");
        //voice.Speak(text, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
    }

    private void SuggestRecoveryTips()
    {
        string text = "Recovery is just as important as training. Here are some tips: " +
                  "Stay hydrated. Stretch for 10-15 minutes after your workout. Get at least 7-8 hours of sleep. Consider foam rolling to relax sore muscles.";
        AddToChat($"Trainer: {text}");
        //voice.Speak(text, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
    }

    private void ProvideMotivation()
    {
        string text = "Remember, consistency is key! Every small step counts towards your goal. Stay strong and keep moving!";
        AddToChat($"Trainer: {text}");
        //voice.Speak(text, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
    }
    private void AddToChat(string message)
    {
        GameObject newText = Instantiate(newTextPrefab, content);
        newText.GetComponent<GeminiTypewriter>().StartTyping(message);

        Canvas.ForceUpdateCanvases();

        scrollRect.verticalNormalizedPosition = 0;
    }

    private void AddPlayerChat(string message)
    {
        GameObject newText = Instantiate(newTextPrefab, content);
        newText.GetComponent<TMP_Text>().text = message;

        Canvas.ForceUpdateCanvases();

        scrollRect.verticalNormalizedPosition = 0;
    }

    private IEnumerator SendToChatGPT(string prompt)
    {
        string jsonPayload = "{\"contents\":[{\"parts\":[{\"text\":\"" + prompt + "\"}]}]}";

        UnityWebRequest request = new UnityWebRequest(apiUrl + "=" + apiKey, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            Debug.Log(response);

            ChatGPTResponse responseFinal = JsonUtility.FromJson<ChatGPTResponse>(response);

            string text = responseFinal.candidates[0].content.parts[0].text;

            AddToChat($"Trainer: {text}");
            //voice.Speak(text, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            string text = "Sorry, something went wrong.";
            AddToChat($"Trainer: {text}");
            //voice.Speak(text, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        }
    }

    [System.Serializable]
    public class ChatGPTResponse
    {
        public Candidate[] candidates;
        public UsageMetadata usageMetadata;
    }

    [System.Serializable]
    public class Candidate
    {
        public Content content;
        public string finishReson;
        public float avgLogprobs;
    }

    [System.Serializable]
    public class Content
    {
        public Part[] parts;
        public string role;
    }

    [System.Serializable]
    public class Part
    {
        public string text;
    }

    [System.Serializable]
    public class UsageMetadata 
    {
        public int promptTokenCount;
        public int candidatesTokenCount;
        public int totalTokenCount;
    }
}
