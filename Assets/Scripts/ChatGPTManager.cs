using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;
using Oculus.Voice.Dictation;
using UnityEngine.InputSystem;

public class ChatGPTManager : MonoBehaviour
{
    [TextArea(5,20)]
    public string personality;
    [TextArea(5, 20)]
    public string scene;
    public int maxResponseWordLimit = 50;

    public InputActionProperty talkInputSource;

    public List<NPCAction> actions;

    public AppDictationExperience voiceToText;

    [System.Serializable]
    public struct NPCAction
    {
        public string actionKeyword;
        [TextArea(2,5)]
        public string actionDescription;

        public UnityEvent actionEvent;
    }

    public OnResponseEvent OnResponse;

    [System.Serializable]

    public class OnResponseEvent : UnityEvent<string> { }

    private OpenAIApi openAI = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();

    public string GetInstructions()
    {
        string instructions = "You are an NPC in a video game and will answer to the message the player asks you. \n" +
            "You must reply to the player message only using the information from your Personality and the Scene that are available to you \n" +
            "Do not invent or create response that are not mentioned in these information. \n" +
            "Never mention that you are an AI or a video game character. Always act like a real person in a real world" +

            "You must answer in less than " + maxResponseWordLimit + "words. \n" +

            "Here is the information about your Personality : \n" +
            personality + "\n" +

            "Here is the information about the Scene around you : \n" +
            scene + "\n" +

            BuildActionInstructions() +

            "Here is the message of the player : \n";
        
        return instructions;
    }

    public string BuildActionInstructions()
    {
        string instructions = "";

        foreach (var item in actions)
        {
            instructions += "if I imply that I want you to do the following : " + item.actionDescription
                + ". You must add to your answer the following keyword : " + item.actionKeyword + ". \n";
        }

        return instructions;
    }

    public async void AskChatGPT(string newText)
    {
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = GetInstructions() + newText;
        newMessage.Role = "user";

        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";

        var response = await openAI.CreateChatCompletion(request);

        if(response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;

            foreach (var item in actions)
            {
                if(chatResponse.Content.Contains(item.actionKeyword))
                {
                    string textNoKeyword = chatResponse.Content.Replace(item.actionKeyword, "");
                    chatResponse.Content = textNoKeyword;
                    item.actionEvent.Invoke();
                }
            }
            messages.Add(chatResponse);

            Debug.Log(chatResponse.Content);

            OnResponse.Invoke(chatResponse.Content);
        }

    }
    
    // Start is called before the first frame update
    void Start()
    {
        voiceToText.DictationEvents.OnFullTranscription.AddListener(AskChatGPT); 
    }

    // Update is called once per frame
    void Update()
    {
        bool talkInput = talkInputSource.action.WasPressedThisFrame();
        if(talkInput)
        {
            voiceToText.Activate();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            voiceToText.Activate();
        }
    }
}
