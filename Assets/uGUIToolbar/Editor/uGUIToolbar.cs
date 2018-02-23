using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class uGUIToolbar : EditorWindow, IHasCustomMenu
{
	private class Data
	{
		public readonly Type m_type;
		public readonly string m_commandRoot;
		public readonly string m_commandName;

		public Data() { }

		public Data( Type type, string commandRoot, string commandName )
		{
			m_type = type;
			m_commandRoot = commandRoot;
			m_commandName = commandName;
		}
	}

	private const string TITLE = "uGUI";
	private const float WINDOW_WIDTH = 32;
	private const float WINDOW_HEIGHT = 28;

	private static readonly GUILayoutOption[] BUTTON_OPTIONS =
	{
		GUILayout.MinWidth( 28 ),
		GUILayout.MaxWidth( 48 ),
		GUILayout.Height( 24 ),
	};

	private static readonly Data[] DATA_LIST =
	{
		new Data( typeof( GameObject    ), "GameObject/"    , "Create Empty Child"  ),
		new Data( typeof( Text          ), "GameObject/UI/" , "Text"                ),
		new Data( typeof( Image         ), "GameObject/UI/" , "Image"               ),
		new Data( typeof( RawImage      ), "GameObject/UI/" , "Raw Image"           ),
		new Data( typeof( Button        ), "GameObject/UI/" , "Button"              ),
		new Data( typeof( Toggle        ), "GameObject/UI/" , "Toggle"              ),
		new Data( typeof( Slider        ), "GameObject/UI/" , "Slider"              ),
		new Data( typeof( Scrollbar     ), "GameObject/UI/" , "Scrollbar"           ),
		new Data( typeof( Dropdown      ), "GameObject/UI/" , "Dropdown"            ),
		new Data( typeof( InputField    ), "GameObject/UI/" , "Input Field"         ),
		new Data( typeof( Canvas        ), "GameObject/UI/" , "Canvas"              ),
		new Data( typeof( ScrollRect    ), "GameObject/UI/" , "Scroll View"         ),
		new Data(),
		new Data( typeof( Shadow        ), "Component/UI/Effects/"  , "Shadow"          ),
		new Data( typeof( Outline       ), "Component/UI/Effects/"  , "Outline"         ),
		new Data( typeof( PositionAsUV1 ), "Component/UI/Effects/"  , "Position As UV1" ),
		new Data( typeof( Mask          ), "Component/UI/"          , "Mask"            ),
		new Data( typeof( RectMask2D    ), "Component/UI/"          , "Rect Mask 2D"    ),
	};

	private uGUIToolbarSettings m_settings;
	private uGUIToolbarSettings Settings
	{
		get { return m_settings ?? ( m_settings = AssetDatabase.LoadAssetAtPath<uGUIToolbarSettings>( "Assets/uGUIToolbar/Editor/uGUIToolbarSettings.asset" ) ); }
	}
	private bool IsVertical
	{
		get { return Settings.m_isVertical; }
		set { Settings.m_isVertical = value; }
	}

	[MenuItem( "Window/uGUI Toolbar" )]
	private static void Init()
	{
		var win = GetWindow<uGUIToolbar>( TITLE );

		var pos = win.position;
		pos.width = 640;
		pos.height = WINDOW_HEIGHT;
		win.position = pos;

		win.minSize = new Vector2( WINDOW_WIDTH, WINDOW_HEIGHT );

		var maxSize = win.maxSize;
		maxSize.y = WINDOW_HEIGHT;
		win.maxSize = maxSize;
	}

	private void OnGUI()
	{
		if ( IsVertical )
		{
			GUILayout.BeginVertical();
		}
		else
		{
			GUILayout.BeginHorizontal();
		}
		foreach ( var n in DATA_LIST )
		{
			var type = n.m_type;
			if ( type == null )
			{
				var options = IsVertical
					? new[] { GUILayout.MinWidth( 28 ), GUILayout.MaxWidth( 48 ), GUILayout.Height( 1 ) }
					: new[] { GUILayout.Height( 24 ), GUILayout.Width( 1 ) }
				;
				GUILayout.Box( string.Empty, options );
				continue;
			}
			var commandName = n.m_commandName;
			var src = EditorGUIUtility.ObjectContent( null, type );
			var content = new GUIContent( src );
			content.text = string.Empty;
			content.tooltip = commandName;
			if ( content == null ) continue;
			var image = content.image;
			if ( !GUILayout.Button( content, BUTTON_OPTIONS ) ) continue;
			var commandRoot = n.m_commandRoot;
			var menuItemPath = string.Format( "{0}{1}", commandRoot, commandName );
			EditorApplication.ExecuteMenuItem( menuItemPath );
		}
		if ( IsVertical )
		{
			GUILayout.EndVertical();
		}
		else
		{
			GUILayout.EndHorizontal();
		}
	}

	public void AddItemsToMenu( GenericMenu menu )
	{
		menu.AddItem( new GUIContent( "Vertical" ), IsVertical, () =>
		{
			IsVertical = !IsVertical;
		} );
	}
}
