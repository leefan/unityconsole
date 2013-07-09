using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

public class Console : MonoBehaviour {
  //console preferences and other details
	protected int Width = 600, Height = 400;		//Console size
	protected string Header = "Console";			//First thing shown when console starts
	protected KeyCode ActivationKey = KeyCode.F2;	//Key used to show/hide console
	protected bool showConsole = false;				//Whether or not console is visible

	
	//Public variables for writing to console stdout and stdin
	public string In;
	public ConsoleWriter Out;
	
	protected static int IDCount = 0;
	protected Rect consoleRect;			//Defines console size and dimensions
	
	int ID;								//unique generated ID
	string ConsoleID;					//generated from ID
	string consoleBuffer;				//Tied to GUI.Label
	Vector2 scrollPosition;
	bool firstFocus;					//Controls console input focus
	
	/// <summary>
	/// Initializes console properties and sets up environment.
	/// </summary>
	void Start () {
		Initialize ();
		consoleBuffer = Header;
		In = string.Empty;
		Out = new ConsoleWriter();
		Out.WriteLine (consoleBuffer);
		scrollPosition = Vector2.zero;
		consoleRect = new Rect(0, 0, Width, Height);
		ID = IDCount++;
		ConsoleID = "window" + ID;
		firstFocus = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Creates the Console Window.
	/// </summary>
	/// <param name='windowID'>
	/// unused parameter.
	/// </param>
	void doConsoleWindow(int windowID) {
		//Console Window
        GUI.DragWindow(new Rect(0, 0, consoleRect.width, 20));
		//Scroll Area
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, 
			GUILayout.MaxHeight(consoleRect.height-48), 
			GUILayout.ExpandHeight (false), 
			GUILayout.Width(consoleRect.width-15));
		//Console Buffer
		GUILayout.Label (consoleBuffer, GUILayout.ExpandHeight (true));
		GUILayout.EndScrollView ();
		//Input Box
		GUI.SetNextControlName(ConsoleID);
		In = GUI.TextField(new Rect(4, consoleRect.height-24, consoleRect.width-8, 20), In);	
		if(firstFocus){
			GUI.FocusControl(ConsoleID);
			firstFocus = false;
		}	
    }
	
    void OnGUI() {
		if(showConsole){
			consoleRect = GUI.Window(ID, consoleRect, doConsoleWindow, Header);
		}
		if (Event.current.isKey && Event.current.keyCode == ActivationKey && Event.current.type == EventType.KeyUp){
			showConsole = !showConsole;
			firstFocus = true;
		}	
		if (Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == ConsoleID && In!=string.Empty) {
			scrollPosition = GUI.skin.label.CalcSize (new GUIContent(consoleBuffer));
			string command = In;
			In = string.Empty;
			Run(command);
		}
		if (Out.isUpdated()){
			consoleBuffer = Out.getTextUpdate();
		}
    }
	
	/// <summary>
	/// A TextWriter for output buffer
	/// </summary>
	public class ConsoleWriter : TextWriter{
		private bool bufferUpdated;			//tracks when changes are made to StringBuilder to prevent generating new strings every click
		private StringBuilder oBuffer;
		public ConsoleWriter ()
		{
			oBuffer = new StringBuilder();
		}

		public override Encoding Encoding {
			get {
				return null;
			}
		}
		public override void Write (string value){
			oBuffer.Append (value);
			bufferUpdated = true;
		}

		public override void WriteLine (string value){
			oBuffer.AppendLine(value);
			bufferUpdated = true;
		}
		
		public string getTextUpdate(){
			bufferUpdated = false;
			return oBuffer.ToString();
		}
		
		public bool isUpdated(){
			return bufferUpdated;
		}
	}
	
	/// <summary>
	/// Run when a newline is entered in the input box.
	/// </summary>
	/// <param name='command'>
	/// The entered text prior to the newline.
	/// </param>
	protected virtual void Run(string command){
		Out.WriteLine (">> "+command);
		//override for functionality
	}
	
	/// <summary>
    /// Allows for initialization of <code>Width</code>, <code>Height</code>, <code>Header</code>, <code>ActivationKey</code>, and <code>showConsole</code>
    /// </summary>
	protected virtual void Initialize(){
		//override to set console properties
	}
}
