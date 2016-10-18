using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ZFrame.Asset
{
	public sealed class AssetBundleLoader : AssetLoader
	{
		private static AssetBundleLoader m_Inst;
		public static AssetBundleLoader Instance { get { return m_Inst; } }

		private void Awake()
		{
			m_Inst = this;
		}

		static string m_persistentDataPath;
		static public string persistentDataPath { get { 
				if (m_persistentDataPath == null) {
#if UNITY_EDITOR || UNITY_STANDALONE
					m_persistentDataPath = Path.GetDirectoryName(Application.dataPath) + "/Issets/PersistentData";
#else
					m_persistentDataPath = Application.persistentDataPath;
#endif
				}
				return m_persistentDataPath;
			}
		}
		static string m_streamingAssetPath;
		static public string streamingAssetsPath { get { 
				if (m_streamingAssetPath == null) {
#if UNITY_EDITOR
					m_streamingAssetPath = Path.GetDirectoryName(Application.dataPath)  + "/Issets/StreamingAssets";
#else
					m_streamingAssetPath = Application.streamingAssetsPath;
#endif
	            }
				return m_streamingAssetPath;
			}
		}

#if UNITY_EDITOR
		public const string DIR_ASSETS = "RefAssets";
#endif

		public const string ASSETBUNDLE_FOLDER = "AssetBundles";
		public const string FILE_LIST = "filelist.bytes";
		static string m_bundleRoot;
		static public string bundleRootPath {
			get {
				if (m_bundleRoot == null) {
					m_bundleRoot = Path.Combine(persistentDataPath, ASSETBUNDLE_FOLDER);
				}
				return m_bundleRoot;
			}
		}

		static string m_streamingRoot;
		static public string streamingRootPath {
			get {
				if (m_streamingRoot == null) {
					m_streamingRoot = Path.Combine(streamingAssetsPath, ASSETBUNDLE_FOLDER);
				}
				return m_streamingRoot;
			}
		}

		private class AssetBundleRef : AbstractAssetBundleRef
		{
			private AssetBundle m_Assetbundle;

	        public AssetBundleRef(string assetbundleName, AssetBundle assetbundle, bool allowUnload)
	        {
                Init(assetbundleName, assetbundle, allowUnload);
	        }
            public void Init(string assetbundleName, AssetBundle assetbundle, bool allowUnload)
	        {
				this.m_Assetbundle = assetbundle;
	            this.name = assetbundleName;
                this.allowUnload = allowUnload;                
	        }
	        public override bool IsEmpty()
	        {
                return m_Assetbundle == null;
	        }

			protected override void UnloadAssets()
			{
				if (m_Assetbundle) {
					m_Assetbundle.Unload(true);
				}
			}

			public override Object Load(string objName, System.Type type)
	        {
                return m_Assetbundle && type != null ? m_Assetbundle.LoadAsset(objName, type) : null;
	        }

			public override IEnumerator LoadAsync(string objName, System.Type type, ObjectOut output)
			{
				if (m_Assetbundle && type != null) {
					var req = m_Assetbundle.LoadAssetAsync(objName, type);
					while (!req.isDone) yield return null;
					output.loadedObj = req.asset;
				}
			}
		};

		public Slider progressView;

		protected override IEnumerator PerformTask(AsyncLoadingTask task, ABRefOut output)
		{
			string suitPath = "";
			string abName = task.assetbundleName;
			if (!task.forcedStreaming) {
				suitPath = Path.Combine(bundleRootPath, abName);
			}
			if (!File.Exists(suitPath)) {
				suitPath = Path.Combine(streamingRootPath, abName);
			}
			if (!suitPath.Contains("://")) {
				if (suitPath.Contains(":")) {
					suitPath = "file:///" + suitPath;
				} else {
					suitPath = "file://" + suitPath;
				}
			}

			WWW www = new WWW(suitPath);
			yield return www;

			if (www.error == null) {
                // 把资源缓存到persistentDataPath
                output.transfer = new AssetsTransfer(www.bytes, Path.Combine(bundleRootPath, abName));
				
				if (task.needMD5) {
					output.md5 = CMD5.MD5Data(www.bytes);
				}                        
				AssetBundle ab = www.assetBundle;
				if (ab != null) {
					//// 资源是一个AssetBundle，获取Bundle里的所有Assets, 保存
					//AssetBundleRequest abReq = ab.LoadAllAssetsAsync();
					//while (!abReq.isDone) {
					//    if (progressView && progressView.gameObject.activeInHierarchy) {
					//        progressView.value = abReq.progress;
					//    }
					//    yield return 1;
					//}
					//if (progressView && progressView.gameObject.activeInHierarchy) {
					//    progressView.value = 1;
					//}
					
					if (output.abRef == null) {
						output.abRef = new AssetBundleRef(abName, ab, task.allowUnload);
					} else {
						var abRef = output.abRef as AssetBundleRef;
						abRef.Init(abName, ab, task.allowUnload);
					}
				}
			}
		}

	    /// <summary>
	    /// 开始加载一个AssetBundle, 只会从streamingRootPath目录下加载
	    /// </summary>
	    /// <param name="path">AssetBundle文件的路径，相对于streamingRootPath根目录</param>
	    /// <param name="allowUnload">是否运行完全卸载</param>
	    /// <param name="onLoaded">加载完成回调处理</param>
	    /// <param name="needMD5">是否需要计算文件的MD5值</param>
		public void StreamingTask(string assetbundleName, bool allowUnload, DelegateAssetBundleLoaded onLoaded = null, bool needMD5 = false)
	    {
			var task = m_TaskPool.Get();
			task.SetBundle(assetbundleName.ToLower(), allowUnload, onLoaded, needMD5);
			task.forcedStreaming = true;
			ScheduleTask(task);
	    }

	    public override  AsyncOperation LoadLevelAsync(string path)
	    {
	        // 解析出资源包和资源对象
	        string assetbundleName, assetName;
	        GetAssetpath(path, out assetbundleName, out assetName);

	        return SceneManager.LoadSceneAsync(assetName);
	    }

		private void OnApplicationQuit()
		{
			AssetsTransfer.StopTransfer();
		}
	    public IEnumerator TransferAll()
	    {
			yield return null;
	#if false
	        if (!AssetsTransfer.IsLock && !AssetsTransfer.IsTransfering && File.Exists(bundleListPath)) {
	            string[] lines = File.ReadAllLines(bundleListPath);
	            List<System.Object> liTransfer = new List<System.Object>();
	            for (int i = 1; i < lines.Length; ++i) {
	                string assetName = lines[i];
	                string bundlePath = Path.Combine(bundleRootPath, assetName);
	                if (!File.Exists(bundlePath)) {
	                    liTransfer.Add(assetName);
	                }
	            }

	            if (liTransfer.Count > 0) {
	                Log(string.Format("Start Transfer {0}", liTransfer.Count));
	                AssetsTransfer.Total = liTransfer.Count;
	                AssetsTransfer.Count = 0;
	                foreach (string asset in liTransfer) {
	                    string url = Path.Combine(streamingRootPath, asset);
	                    if (!url.Contains("://")) {
	                        url = "file://" + url;
	                    }
	                    WWW www = new WWW(url);
	                    yield return www;
	        
	                    if (www.error == null) {
	                        new AssetsTransfer(www.bytes, Path.Combine(bundleRootPath, asset));
	                    }
	                    AssetsTransfer.Count++;
	                }
	            }
	        }
	#endif
	    }

	}
}
