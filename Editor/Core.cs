using HananokiEditor.Extensions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using E = HananokiEditor.AutoBackup.SettingsProject;
using SS = HananokiEditor.SharedModule.S;
using UnityScene = UnityEngine.SceneManagement.Scene;


namespace HananokiEditor.AutoBackup {
	[InitializeOnLoad]
	public static class Core {

		internal static string backupDomain = $"{Package.reverseDomainName}-temp";
		internal static string バックアップ先のアセットパス = $"Packages/{backupDomain}";

		internal static double s_elapsedTime;


		static Core() {
			E.Load();
			E.i.有効 = AssetDatabase.IsValidFolder( バックアップ先のアセットパス );

			if( !E.i.有効 ) return;

			Start();
		}


		/////////////////////////////////////////
		public static void Start() {
			s_elapsedTime = EditorApplication.timeSinceStartup;

			EditorApplication.update -= OnUpdate;
			EditorApplication.update += OnUpdate;

			EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

			EditorSceneManager.sceneOpened -= OnSceneOpened;
			EditorSceneManager.sceneOpened += OnSceneOpened;

			Utils.指定ファイル数を超えたら古いファイルを消す();
		}


		/////////////////////////////////////////
		public static void Exit() {
			EditorApplication.update -= OnUpdate;
			EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
			EditorSceneManager.sceneOpened -= OnSceneOpened;

		}


		/////////////////////////////////////////
		static void OnSceneOpened( UnityScene scene, OpenSceneMode mode ) {
			if( !E.i.有効 ) return;

			s_elapsedTime = EditorApplication.timeSinceStartup;
		}


		/////////////////////////////////////////
		public static void OnUpdate() {
			if( !E.i.有効 ) return;

			if( Application.isPlaying ) return;

			if( !E.i.m_backupPackageName.IsExistsDirectory() ) {
				Exit();
				EditorUtility.DisplayDialog( SS._Error, "動作中にバックアップパッケージが消えたため動作を停止します", SS._OK );
				return;
			}

			var sa = EditorApplication.timeSinceStartup - s_elapsedTime;
			if( ( 60.0 * E.i.m_BackupInterval_Minutes ) < sa ) {
				s_elapsedTime = EditorApplication.timeSinceStartup;
				Utils.シーンをバックアップ();
			}
		}



		/////////////////////////////////////////
		static void OnPlayModeStateChanged( PlayModeStateChange state ) {
			if( !E.i.有効 ) return;
			if( !E.i.Backupscenesを退避する ) return;

			// エディットモードの終了時
			if( state == PlayModeStateChange.ExitingEditMode ) {
				// マルチの後シングルにするとマルチ分が残骸で残るので一旦消す
				fs.rm( "Temp/__Backupscenes", true );
				fs.rm( $"{E.i.m_backupPackageName}/__Backupscenes", true );
			}

			// プレイモードの終了時
			if( state == PlayModeStateChange.ExitingPlayMode ) {
			}


			// プレイモードの開始時
			if( state == PlayModeStateChange.EnteredPlayMode ) {
				//if( "Temp/__Backupscenes".IsExistsDirectory() ) {
				fs.cp( "Temp/__Backupscenes", $"{E.i.m_backupPackageName}/__Backupscenes" );
				foreach( var p in DirectoryUtils.GetFiles( $"{E.i.m_backupPackageName}/__Backupscenes" ) ) {
					//Debug.Log(p);
					fs.mv( p, p.Replace( ".backup", ".unity" ) );
				}
			}

			// エディットモードの開始時
			//if( state == PlayModeStateChange.EnteredEditMode ) {
			//}
		}


	}
}

