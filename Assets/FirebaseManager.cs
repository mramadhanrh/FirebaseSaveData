using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

// Re-Code Oleh : Muhammad Ramadhan Rahmat
// Firebase Input Data Example

public class FirebaseManager : MonoBehaviour {

    DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    public int id = 0;
    public Text name, age, hobby;
    private string nameText, hobbyText;
    private int ageText;
    DependencyStatus depedencyStatus = DependencyStatus.UnavailableOther;

    const int kMaxLogSize = 16382;
    private Vector2 scrollViewVector = Vector2.zero;
    private string logText = "";
	// Use this for initialization
	void Start () {
        depedencyStatus = FirebaseApp.CheckDependencies();
        if (depedencyStatus != DependencyStatus.Available)
        {
            FirebaseApp.FixDependenciesAsync().ContinueWith(task =>
            {

                depedencyStatus = FirebaseApp.CheckDependencies();
                if (depedencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();
                    Debug.Log("Depedency ada, Initialize Firebase...");
                }
                else
                {
                    Debug.LogWarning("Could not resolve all Firebase dependencies: " + depedencyStatus);
                }
            });

        }
        else
        {
            InitializeFirebase();
            Debug.Log("Firebase Initialized");
        }
	}

    void InitializeFirebase()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance; //Nge cast FirebaseApp
        app.SetEditorDatabaseUrl("https://fir-test-3a9a6.firebaseio.com/"); //Url Firebase Letakan Disini
    }

    TransactionResult AddUserTransaction(MutableData mutableData)
    {
        List<object> users = mutableData.Value as List<object>;

        if (users == null)
        {
            users = new List<object>();
            Debug.Log("new list initiated");
        }

        Dictionary<string, object> userMap = new Dictionary<string, object>();
        
        userMap["name"] = nameText;
        userMap["age"] = ageText;
        userMap["hobby"] = hobbyText;
        users.Add(userMap);

        mutableData.Value = users;
        return TransactionResult.Success(mutableData);
    }

    public void AddUser()
    {
        nameText = name.GetComponent<Text>().text;
        hobbyText = hobby.GetComponent<Text>().text;
        ageText = Int32.Parse(age.GetComponent<Text>().text);
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("ListDataUser");
        Debug.Log("Dijalankan AddUser() ....");
        reference.RunTransaction(AddUserTransaction) //yang di bawah hanya untuk debug
        .ContinueWith(task =>
        { 
            if (task.Exception != null)
            {
                DebugLog(task.Exception.ToString());
            }
            else if (task.IsCompleted)
            {
                DebugLog("Transaction complete.");
            }
        }); ;
    }

    public void DebugLog(string s)
    {
        Debug.Log(s);
        logText += s + "\n";

        while (logText.Length > kMaxLogSize)
        {
            int index = logText.IndexOf("\n");
            logText = logText.Substring(index + 1);
        }

        scrollViewVector.y = int.MaxValue;
    }
}
