using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BestHTTP;

namespace SuperUltra.Container
{
    public class RankingInfo
    {
        public int rank;
        public Sprite image;
        public string name;
        public int score;
    }

    public class LeaderboardUI : MonoBehaviour
    {
        [SerializeField] RankingItemUI _rankingItemUI;
        [SerializeField] RectTransform _rankingItemContainer;
        [SerializeField] RectTransform _gameBannerPrefab;
        [SerializeField] RectTransform _gameBannerContainer;
        [SerializeField] StickyScrollUI _gameBannerStickyScroll;
        [SerializeField] RankingItemUI _userRankingUI;
        [SerializeField] TMP_Text _gameName;
        [SerializeField] TMP_Text _poolSize;
        [SerializeField] TMP_Text _timeLeft;
        Dictionary<int, List<RankingInfo>> _gameToRankingInfoMap = new Dictionary<int, List<RankingInfo>>();
        int _currentGameId = -1;
        float _listSpacing = 0;
        float _itemHeight = 0;
        Dictionary<int, int> _pageToGameMap = new Dictionary<int, int>();

        // Start is called before the first frame update
        void Start()
        {
            CacheSpacingAndheight();
            CreateGameList();
            CreateUserRank();
        }

        void Update()
        {
            UpdateTournamentInfo(_currentGameId);
        }

        void CreateUserRank()
        {
            if (_userRankingUI == null)
            {
                return;
            }
            LeaderboardUserData userRank = new LeaderboardUserData()
            {
                rankPosition = 45,
                avatarUrl = "",
                name = "LiftTastic",
                score = 608
            };
            _userRankingUI.SetData(userRank);
        }

        void CreateGameList()
        {
            int pageCount = 0;
            Debug.Log($"CreateGameList {GameData.gameDataList.Count}");
            foreach (var game in GameData.gameDataList)
            {
                RectTransform gameBanner = Instantiate(_gameBannerPrefab, _gameBannerContainer);
                gameBanner.GetComponent<Image>().sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
                float value = UnityEngine.Random.Range(0.1f, 0.9f);
                float value1 = UnityEngine.Random.Range(0.1f, 0.9f);
                float value2 = UnityEngine.Random.Range(0.1f, 0.9f);

                gameBanner.GetComponent<Image>().color = new Color(value, value1, value2);
                _pageToGameMap.Add(pageCount, game.Key);
                CreateRankingList(game.Key);
                pageCount++;
            }
            _gameBannerStickyScroll.Initialize();
            _gameBannerStickyScroll.OnItemChange.AddListener(OnGameChange);
        }

        void OnGameChange(float page)
        {
            RefreshLeaderboard(_pageToGameMap[(int)page]);
            _currentGameId = _pageToGameMap[(int)page];
        }

        void CacheSpacingAndheight()
        {
            VerticalLayoutGroup verticalLayoutGroup = _rankingItemContainer.GetComponent<VerticalLayoutGroup>();
            if (verticalLayoutGroup)
            {
                _listSpacing = verticalLayoutGroup.spacing;
            }

            RectTransform itemRect = _rankingItemUI.GetComponent<RectTransform>();
            if (itemRect)
            {
                _itemHeight = itemRect.sizeDelta.y;
            }
        }

        void ClearLeaderBoard()
        {
            foreach (Transform child in _rankingItemContainer)
            {
                Destroy(child.gameObject);
            }
            _rankingItemContainer.sizeDelta = Vector2.zero;
        }

        void CreateRankingList(int gameID = 0)
        {
            Debug.Log($"CreateRankingList {gameID} {GameData.gameDataList[gameID]}");
            if (!GameData.gameDataList.TryGetValue(gameID, out GameData gameData))
            {
                return;
            }

            Debug.Log($"CreateRankingList {gameData.leaderboard.Count}");
            foreach (var item in gameData.leaderboard)
            {
                RankingItemUI rankingItemUI = Instantiate(_rankingItemUI, _rankingItemContainer);
                rankingItemUI.SetData(item);
                _rankingItemContainer.sizeDelta += new Vector2(
                    0,
                    _itemHeight + _listSpacing
                );
            }
        }

        public void RefreshLeaderboard(int gameID = 0)
        {
            ClearLeaderBoard();
            CreateRankingList(gameID);
        }

        public void UpdateTournamentInfo(int gameId)
        {
            if (!GameData.gameDataList.TryGetValue(gameId, out GameData data))
            {
                return;
            }

            if (_gameName != null)
            {
                _gameName.text = data.name;
            }
            if (_poolSize != null)
            {

                _poolSize.text = data.tournament.prizePool.ToString();
            }
            if (_timeLeft != null)
            {
                // calculate time left using tounament.endTime and Date.now()
                TimeSpan timeLeft = data.tournament.endTime - DateTime.Now;
                string timeText = $"{timeLeft.Days}d {timeLeft.Hours}h {timeLeft.Minutes}m {timeLeft.Seconds}s";
                _timeLeft.text = timeText;
            }
        }

    }

}
