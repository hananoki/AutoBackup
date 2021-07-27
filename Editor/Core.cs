#if UNITY_2019_1_OR_NEWER

using HananokiEditor.Extensions;
using HananokiRuntime;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using E = HananokiEditor.AutoBackup.SettingsProject;
using UnityScene = UnityEngine.SceneManagement.Scene;


namespace HananokiEditor.AutoBackup {
	[InitializeOnLoad]
	public static class Core {

		internal static string backupDomain = $"{Package.reverseDomainName}-temp";
		internal static string バックアップ先のアセットパス = $"Packages/{backupDomain}";

		internal static double s_最後に保存した時間;
		internal static double s_フォルダ監視する時間;

		internal static Confirm s_confirm;

		internal static string s_rollbackGUID;


		static Core() {
			E.Load();
			E.i.有効 = AssetDatabase.IsValidFolder( バックアップ先のアセットパス );

			if( !E.i.有効 ) return;

			Start();
		}


		/////////////////////////////////////////
		public static void Start() {
			s_最後に保存した時間 = EditorApplication.timeSinceStartup;
			s_フォルダ監視する時間 = EditorApplication.timeSinceStartup;

			EditorApplication.update -= HananokiEditor_AutoBackup_Core_Update;
			EditorApplication.update += HananokiEditor_AutoBackup_Core_Update;

			EditorApplication.playModeStateChanged -= HananokiEditor_AutoBackup_Core_PlayModeStateChanged;
			EditorApplication.playModeStateChanged += HananokiEditor_AutoBackup_Core_PlayModeStateChanged;

			EditorSceneManager.sceneOpened -= HananokiEditor_AutoBackup_Core_SceneOpened;
			EditorSceneManager.sceneOpened += HananokiEditor_AutoBackup_Core_SceneOpened;

			Utils.指定ファイル数を超えたら古いファイルを消す();
		}


		/////////////////////////////////////////
		public static void Exit() {
			EditorApplication.update -= HananokiEditor_AutoBackup_Core_Update;
			EditorApplication.playModeStateChanged -= HananokiEditor_AutoBackup_Core_PlayModeStateChanged;
			EditorSceneManager.sceneOpened -= HananokiEditor_AutoBackup_Core_SceneOpened;
		}


		/////////////////////////////////////////
		static void HananokiEditor_AutoBackup_Core_SceneOpened( UnityScene scene, OpenSceneMode mode ) {
			if( !E.i.有効 ) return;

			s_最後に保存した時間 = EditorApplication.timeSinceStartup;

			Helper.New( ref s_confirm );
			s_confirm.RemoveUI();

			var mm = Regex.Matches( scene.path, @".*\s\[(.+)\]\.unity" );
			if( 0 < mm.Count ) {
				s_rollbackGUID = mm[ 0 ].Groups[ 1 ].Value;


				s_confirm.Attach();
			}
		}


		/////////////////////////////////////////
		public static void HananokiEditor_AutoBackup_Core_Update() {

			if( !E.i.有効 ) return;
			if( Application.isPlaying ) return;

			// Directory.Existsするだけで714ByteのGCAllocする、what is this?
			// 毎回チェックするほど重要ではないので10秒に一回程度にする
			var 差 = EditorApplication.timeSinceStartup - s_フォルダ監視する時間;
			if( ( 10.0 ) < 差 ) {
				if( !E.i.m_backupPackageName.IsExistsDirectory() ) {
					Exit();
					HEditorDialog.Error( S._Itwillstopworkingbecausethebackuppackagedisappearedduringoperation );
					return;
				}
				s_フォルダ監視する時間 = EditorApplication.timeSinceStartup;
			}

			var sa = EditorApplication.timeSinceStartup - s_最後に保存した時間;
			if( ( 60.0 * E.i.m_BackupInterval_Minutes ) < sa ) {
				s_最後に保存した時間 = EditorApplication.timeSinceStartup;
				Utils.シーンをバックアップ();
			}
		}



		/////////////////////////////////////////
		static void HananokiEditor_AutoBackup_Core_PlayModeStateChanged( PlayModeStateChange state ) {
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

#endif
