using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Linq;
using TerrorConsole;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Robot
{
    public class UIManager : Singleton<IUISource>, IUISource
    {
        [SerializeField] private GameObject _chipUIInventory, _pauseMenu, _settingsMenu, _shiftControl;
        [SerializeField] private CanvasGroup _guiUI, _backgroundUI;

        [Header("Tutorial Menu")]
        [SerializeField] private GameObject _tutorialMenu;
        [SerializeField] private GameObject _tutorialContext;
        [SerializeField] private GameObject _tutorialControls;
        [SerializeField] private GameObject _tutorialObjective;

        [Header("EndGame")]
        [SerializeField] private CanvasGroup _endgameUI;
        [SerializeField] private CanvasGroup _titleEndgameUI;
        [SerializeField] private CanvasGroup _subtitleEndgameUI;

        public GameObject ChipUI => _chipUIInventory;

        private void Start()
        {
            InputManager.Source.OpenChipsInventory += OpenChipInventory;
            InputManager.Source.CloseChipsInventory += CloseChipInventory;
            InputManager.Source.CloseTutorial += CloseTutorialMenu;
            PlayerStatsManager.Source.OnDashChipActivation += IsDashActivate;
        }

        private void OnDisable()
        {
            InputManager.Source.OpenChipsInventory -= OpenChipInventory;
            InputManager.Source.CloseChipsInventory -= CloseChipInventory;
            InputManager.Source.CloseTutorial -= CloseTutorialMenu;
            PlayerStatsManager.Source.OnDashChipActivation -= IsDashActivate;
        }

        public void OpenPause()
        {
            ShowPauseMenu().Forget();
        }

        public void ClosePause()
        {
            if (_settingsMenu.activeSelf)
            {
                CloseSetting();
                GameStateManager.Source.ChangeState(GameState.OnPause);
            }
            else
            {
                HidePauseMenu().Forget();
            }

        }

        public void OpenTutorialMenu()
        {
            ShowTutorialMenus().Forget();
        }

        private void CloseTutorialMenu()
        {
            HideTutorialMenus().Forget();
        }

        public void OpenSetting()
        {
            ShowSettingsMenu().Forget();
        }

        public void CloseSetting()
        {
            HideSettingsMenu().Forget();
        }

        public void OpenEndgame()
        {
            ShowEndgame().Forget();
        }

        private void CloseChipInventory()
        {
            var chips = FindObjectsByType<ChipUIHandler>(FindObjectsSortMode.None);
            bool isChipDragging = chips.Any(c => c._isDragging);

            if (!isChipDragging)
            {
                _chipUIInventory.SetActive(false);
                ShowGUI();
                GameStateManager.Source.ChangeState(GameState.OnPlay);
            }
        }

        private void OpenChipInventory()
        {
            _chipUIInventory.SetActive(true);
            //_chipUIInventory.GetComponentInChildren<ChipDisplay>().RefreshChipDescription();
            HideGUI();
        }

        private void HideGUI()
        {
            _guiUI.alpha = 0;
        }

        private void ShowGUI()
        {
            _guiUI.alpha = 1;
        }

        public void IsDashActivate(bool value)
        {
            _shiftControl.SetActive(value);
        }


        private async UniTask ShowSettingsMenu()
        {
            await _pauseMenu.transform.DOLocalMoveX(1415,0.5f).AsyncWaitForCompletion();
            _pauseMenu.gameObject.SetActive(false);
            _settingsMenu.gameObject.SetActive(true);
            await _settingsMenu.transform.DOLocalMoveY(0, 0.5f).AsyncWaitForCompletion();
        }

        private async UniTask HideSettingsMenu()
        {
            await _settingsMenu.transform.DOLocalMoveY(1025, 0.5f).AsyncWaitForCompletion();
            _settingsMenu.gameObject.SetActive(false);
            _pauseMenu.gameObject.SetActive(true);
            await _pauseMenu.transform.DOLocalMoveX(0, 0.5f).AsyncWaitForCompletion();
        }

        private async UniTask ShowPauseMenu()
        {
            HideGUI();
            _backgroundUI.gameObject.SetActive(true);
            await _backgroundUI.DOFade(1, 0.5f).AsyncWaitForCompletion();
        }

        private async UniTask HidePauseMenu()
        {
            await _backgroundUI.DOFade(0, 0.5f).AsyncWaitForCompletion();
            ShowGUI();
            _backgroundUI.gameObject.SetActive(false);
        }

        private async UniTask ShowTutorialMenus()
        {
            GameStateManager.Source.ChangeState(GameState.InTransition);
            _guiUI.alpha = 0;
            _tutorialMenu.SetActive(true);
            Sequence sequence = DOTween.Sequence();

            sequence.Append(_tutorialContext.transform.DOLocalMoveX(-800, 1f)); 
            sequence.AppendInterval(1);
            sequence.Append(_tutorialControls.transform.DOLocalMoveY(0, 1f)); 
            sequence.AppendInterval(1);
            sequence.Append(_tutorialObjective.transform.DOLocalMoveX(800, 1f)); 
            sequence.AppendCallback(() => GameStateManager.Source.ChangeState(GameState.OnTutorial));
            await sequence.AsyncWaitForCompletion();
        }

        private async UniTask HideTutorialMenus()
        {
            GameStateManager.Source.ChangeState(GameState.InTransition);
            Sequence sequence = DOTween.Sequence();

            sequence.Append(_tutorialContext.transform.DOLocalMoveX(-1300, 1f));
            sequence.Append(_tutorialControls.transform.DOLocalMoveY(1080, 1f)); 
            sequence.Append(_tutorialObjective.transform.DOLocalMoveX(1300, 1f)); 
            sequence.AppendCallback(() => GameStateManager.Source.ChangeState(GameState.OnPlay));
            await sequence.AsyncWaitForCompletion();
            _guiUI.alpha = 1;
            _tutorialMenu.SetActive(false);
        }

        private async UniTask ShowEndgame()
        {
            GameStateManager.Source.ChangeState(GameState.InTransition);
            _endgameUI.gameObject.SetActive(true);
            Sequence sequence = DOTween.Sequence();

            sequence.Append(_endgameUI.DOFade(1, 1f));
            sequence.Append(_titleEndgameUI.DOFade(1, 1f));
            sequence.Append(_subtitleEndgameUI.DOFade(1, 1f));
            sequence.Append(_titleEndgameUI.DOFade(0, 1f));
            sequence.Join(_subtitleEndgameUI.DOFade(0, 1f));
            await sequence.AsyncWaitForCompletion();

            SceneManager.LoadScene("Menu Inicial");
        }
    }
}
