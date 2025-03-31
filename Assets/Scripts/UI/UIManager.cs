using Patterns;
using System.Collections.Generic;
using UnityEngine;


namespace Game.UI
{
    public class UIManager : Singleton<UIManager>
    {
        public GameObject cScreen, cPopup, cNotify, cOverlap;

        private Dictionary<string, BaseScreen> _screens = new Dictionary<string, BaseScreen>();
        private Dictionary<string, BasePopup> _popups = new Dictionary<string, BasePopup>();
        private Dictionary<string, BaseNotify> _notifies = new Dictionary<string, BaseNotify>();
        private Dictionary<string, BaseOverlap> _overlaps = new Dictionary<string, BaseOverlap>();

        public Dictionary<string, BaseScreen> Screens => _screens;
        public Dictionary<string, BasePopup> Popups => _popups;
        public Dictionary<string, BaseNotify> Notifies => _notifies;
        public Dictionary<string, BaseOverlap> Overlaps => _overlaps;

        private BaseScreen _curScreen;
        private BasePopup _curPopup;
        private BaseNotify _curNotify;
        private BaseOverlap _curOverlap;

        public BaseScreen CurScreen => _curScreen;
        public BasePopup CurPopup => _curPopup;
        public BaseNotify CurNotify => _curNotify;
        public BaseOverlap CurOverlap => _curOverlap;

        private const string SCREEN_RESOURCES_PATH = "Prefabs/UI/Screen/";
        private const string POPUP_RESOURCES_PATH = "Prefabs/UI/Popup/";
        private const string NOTIFY_RESOURCES_PATH = "Prefabs/UI/Notify/";
        private const string OVERLAP_RESOURCES_PATH = "Prefabs/UI/Overlap/";


        #region Screen

        private BaseScreen GetNewScreen<T>() where T : BaseScreen
        {
            string nameScreen = typeof(T).Name;
            GameObject pfScreen = GetUIPrefab(UIType.Screen, nameScreen);
            if (pfScreen == null || !pfScreen.GetComponent<BaseScreen>())
            {
                throw new MissingReferenceException("Can not found" + nameScreen + "screen. !!!");
            }
            GameObject ob = Instantiate(pfScreen) as GameObject;
            ob.transform.SetParent(this.cScreen.transform);
            ob.transform.localScale = Vector3.one;
            ob.transform.localPosition = Vector3.zero;
#if UNITY_EDITOR
            ob.name = "SCREEN_" + nameScreen;
#endif
            BaseScreen screenScr = ob.GetComponent<BaseScreen>();
            screenScr.Init();
            return screenScr;
        }

        public void HideAllScreens()
        {
            BaseScreen screenScr = null;

            foreach (KeyValuePair<string, BaseScreen> item in _screens)
            {
                screenScr = item.Value;
                if (screenScr == null || screenScr.IsHide)
                    continue;
                screenScr.Hide();

                if (_screens.Count <= 0)
                    break;
            }
        }

        public T GetExistScreen<T>() where T : BaseScreen
        {
            string screenName = typeof(T).Name;
            if (_screens.ContainsKey(screenName))
            {
                return _screens[screenName] as T;
            }
            return null;
        }


        public void ShowScreen<T>(object data = null, bool forceShowData = false) where T : BaseScreen
        {
            string screenName = typeof(T).Name;
            BaseScreen result = null;
            if (_curScreen != null)
            {
                var curName = _curScreen.GetType().Name;
                if (curName.Equals(screenName))
                {
                    result = _curScreen;
                }
                else
                {
                    _screens[curName].Hide();
                }
            }

            if (result == null)
            {
                if (!_screens.ContainsKey(screenName))
                {
                    BaseScreen screenScr = GetNewScreen<T>();
                    if (screenScr != null)
                    {
                        _screens.Add(screenName, screenScr);
                    }
                }

                if (_screens.ContainsKey(screenName))
                {
                    result = _screens[screenName];
                }
            }

            bool isShow = false;
            if (result != null)
            {
                if (forceShowData)
                {
                    isShow = true;
                }
                else
                {
                    if (result.IsHide)
                    {
                        isShow = true;
                    }
                }
            }

            if (isShow)
            {
                _curScreen = result;
                result.transform.SetAsLastSibling();
                result.Show(data);
            }
        }

        #endregion

        #region Popup

        private BasePopup GetNewPopup<T>() where T : BasePopup
        {
            string namePopup = typeof(T).Name;
            GameObject pfPopup = GetUIPrefab(UIType.Popup, namePopup);
            if (pfPopup == null || !pfPopup.GetComponent<BasePopup>())
            {
                throw new MissingReferenceException("Can not found" + namePopup + "popup. !!!");
            }
            GameObject ob = Instantiate(pfPopup) as GameObject;
            ob.transform.SetParent(this.cPopup.transform);
            ob.transform.localScale = Vector3.one;
            ob.transform.localPosition = Vector3.zero;
#if UNITY_EDITOR
            ob.name = "POPUP_" + namePopup;
#endif
            BasePopup popupScr = ob.GetComponent<BasePopup>();
            popupScr.Init();
            return popupScr;
        }

        public void HideAllPopups()
        {
            BasePopup popupScr = null;

            foreach (KeyValuePair<string, BasePopup> item in _popups)
            {
                popupScr = item.Value;
                if (popupScr == null || popupScr.IsHide)
                    continue;
                popupScr.Hide();

                if (_popups.Count <= 0)
                    break;
            }
        }

        public T GetExistPopup<T>() where T : BasePopup
        {
            string popupName = typeof(T).Name;
            if (_popups.ContainsKey(popupName))
            {
                return _popups[popupName] as T;
            }
            return null;
        }

        public void ShowPopup<T>(object data = null, bool forceShowData = false) where T : BasePopup
        {
            string popupName = typeof(T).Name;
            BasePopup result = null;

            if (_curPopup != null)
            {
                var curName = _curPopup.GetType().Name;
                if (curName.Equals(popupName))
                {
                    result = _curPopup;
                }
                else
                {
                    _popups[curName].Hide();
                }
            }

            if (result == null)
            {
                if (!_popups.ContainsKey(popupName))
                {
                    BasePopup popupScr = GetNewPopup<T>();
                    if (popupScr != null)
                    {
                        _popups.Add(popupName, popupScr);
                    }
                }

                if (_popups.ContainsKey(popupName))
                {
                    result = _popups[popupName];
                }
            }


            if (result != null && (forceShowData || result.IsHide))
            {
                _curPopup = result;
                result.transform.SetAsLastSibling();
                (result as T).Show(data);
            }
        }

        #endregion

        #region Notify

        private BaseNotify GetNewNotify<T>() where T : BaseNotify
        {
            string nameNotify = typeof(T).Name;
            GameObject pfNotify = GetUIPrefab(UIType.Notify, nameNotify);
            if (pfNotify == null || !pfNotify.GetComponent<BaseNotify>())
            {
                throw new MissingReferenceException("Can not found" + nameNotify + "notify. !!!");
            }
            GameObject ob = Instantiate(pfNotify) as GameObject;
            ob.transform.SetParent(this.cNotify.transform);
            ob.transform.localScale = Vector3.one;
            ob.transform.localPosition = Vector3.zero;
#if UNITY_EDITOR
            ob.name = "NOTIFY_" + nameNotify;
#endif
            BaseNotify notifyScr = ob.GetComponent<BaseNotify>();
            notifyScr.Init();
            return notifyScr;
        }

        public void HideAllNotifies()
        {
            BaseNotify notifyScr = null;

            foreach (KeyValuePair<string, BaseNotify> item in _notifies)
            {
                notifyScr = item.Value;
                if (notifyScr == null || notifyScr.IsHide)
                    continue;
                notifyScr.Hide();

                if (_notifies.Count <= 0)
                    break;
            }
        }

        public T GetExistNotify<T>() where T : BaseNotify
        {
            string notifyName = typeof(T).Name;
            if (_notifies.ContainsKey(notifyName))
            {
                return _notifies[notifyName] as T;
            }
            return null;
        }


        public void ShowNotify<T>(object data = null, bool forceShowData = false) where T : BaseNotify
        {
            string notifyName = typeof(T).Name;
            BaseNotify result = null;

            if (_curNotify != null)
            {
                var curName = _curPopup.GetType().Name;
                if (curName.Equals(notifyName))
                {
                    result = _curNotify;
                }
                else
                {
                    _notifies[curName].Hide();
                }
            }

            if (result == null)
            {
                if (!_notifies.ContainsKey(notifyName))
                {
                    BaseNotify notifyScr = GetNewNotify<T>();
                    if (notifyScr != null)
                    {
                        _notifies.Add(notifyName, notifyScr);
                    }
                }

                if (_notifies.ContainsKey(notifyName))
                {
                    result = _notifies[notifyName];
                }
            }

            bool isShow = false;
            if (result != null)
            {
                if (forceShowData)
                {
                    isShow = true;
                }
                else
                {
                    if (result.IsHide)
                    {
                        isShow = true;
                    }
                }
            }

            if (isShow)
            {
                _curNotify = result;
                result.transform.SetAsLastSibling();
                result.Show(data);
            }
        }

        #endregion

        #region Overlap

        private BaseOverlap GetNewOverLap<T>() where T : BaseOverlap
        {
            string nameOverlap = typeof(T).Name;
            GameObject pfOverlap = GetUIPrefab(UIType.Overlap, nameOverlap);
            if (pfOverlap == null || !pfOverlap.GetComponent<BaseOverlap>())
            {
                throw new MissingReferenceException("Can not found" + nameOverlap + "overlap. !!!");
            }
            GameObject ob = Instantiate(pfOverlap) as GameObject;
            ob.transform.SetParent(this.cOverlap.transform);
            ob.transform.localScale = Vector3.one;
            ob.transform.localPosition = Vector3.zero;
#if UNITY_EDITOR
            ob.name = "OVERLAP_" + nameOverlap;
#endif
            BaseOverlap overlapScr = ob.GetComponent<BaseOverlap>();
            overlapScr.Init();
            return overlapScr;
        }

        public void HideAllOverlaps()
        {
            BaseOverlap overlapScr = null;

            foreach (KeyValuePair<string, BaseOverlap> item in _overlaps)
            {
                overlapScr = item.Value;
                if (overlapScr == null || overlapScr.IsHide)
                    continue;
                overlapScr.Hide();

                if (_overlaps.Count <= 0)
                    break;
            }
        }

        public T GetExistOverlap<T>() where T : BaseOverlap
        {
            string overlapName = typeof(T).Name;
            if (_overlaps.ContainsKey(overlapName))
            {
                return _overlaps[overlapName] as T;
            }
            return null;
        }

        public void ShowOverlap<T>(object data = null, bool forceShowData = false) where T : BaseOverlap
        {
            string overlapName = typeof(T).Name;
            BaseOverlap result = null;

            if (_curOverlap != null)
            {
                var curName = _curOverlap.GetType().Name;
                if (curName.Equals(overlapName))
                {
                    result = _curOverlap;
                }
                else
                {
                    _overlaps[curName].Hide();
                }
            }

            if (result == null)
            {
                if (!_overlaps.ContainsKey(overlapName))
                {
                    BaseOverlap overlapScr = GetNewOverLap<T>();
                    if (overlapScr != null)
                    {
                        _overlaps.Add(overlapName, overlapScr);
                    }
                }

                if (_overlaps.ContainsKey(overlapName))
                {
                    result = _overlaps[overlapName];
                }
            }

            bool isShow = false;
            if (result != null)
            {
                if (forceShowData)
                {
                    isShow = true;
                }
                else
                {
                    if (result.IsHide)
                    {
                        isShow = true;
                    }
                }
            }

            if (isShow)
            {
                _curOverlap = result;
                result.transform.SetAsLastSibling();
                result.Show(data);
            }
        }

        #endregion

        private GameObject GetUIPrefab(UIType t, string uiName)
        {
            GameObject result = null;
            var defaultPath = "";
            if (result == null)
            {
                switch (t)
                {
                    case UIType.Screen:
                        {
                            defaultPath = SCREEN_RESOURCES_PATH + uiName;
                        }
                        break;
                    case UIType.Popup:
                        {
                            defaultPath = POPUP_RESOURCES_PATH + uiName;
                        }
                        break;
                    case UIType.Notify:
                        {
                            defaultPath = NOTIFY_RESOURCES_PATH + uiName;
                        }
                        break;
                    case UIType.Overlap:
                        {
                            defaultPath = OVERLAP_RESOURCES_PATH + uiName;
                        }
                        break;
                }

                result = Resources.Load(defaultPath) as GameObject;
            }
            return result;
        }

    }
}