using Assets.Scripts.Audio;
using Assets.Scripts.Game.Enemy;
using Assets.Scripts.Game.HealthBar;
using Assets.Scripts.Tutorial;
using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Game
{
	public class GameManager : SingletonMono<GameManager>
	{
		[SerializeField] private CanvasGroup _gameHud;
		[SerializeField] private CanvasGroup _menuUI;
		[SerializeField] private CanvasGroup _game;
		[SerializeField] private HealthBarHolder _healthBarHolder;

		[SerializeField] private Animator _animatorBackgroundSpace;

		[SerializeField] private EnemyManager _enemyManager;

		private int _round = 1;

		public override void Awake()
		{
			base.Awake();

			_healthBarHolder.ResetHealth();

			var isTutorial = Convert.ToBoolean(PlayerPrefs.GetInt("first_tutorial_input", 1));

			if(isTutorial == true)
			{
				TutorialManager.Instance.ShowTutorial();
				PlayerPrefs.SetInt("first_tutorial_input", 0);
			}
			else
			{
				TutorialManager.Instance.CloseTutorial();
			}

			_animatorBackgroundSpace.speed = 10f;

			_enemyManager.Subscribe(OnHandleEndRound);
		}

		private void OnHandleEndRound()
		{
			_round++;
			StartRound();
		}

		private void StartRound()
		{
			_enemyManager.CreateEnemy(_round);
		}

		[ContextMenu("Start Game")]
		public void StartGame()
		{
			_gameHud.alpha = 0f;
			_gameHud.gameObject.SetActive(true);

			_gameHud.DOFade(1f, 0.5f);
			_menuUI.DOFade(0f, 0.5f).onComplete += () =>
			{
				_menuUI.gameObject.SetActive(false);
				_game.DOFade(1f, 0.3f);
				_game.gameObject.SetActive(true);
			};

			AudioManager.Instance.PlayMusic(TypeAudio.MusicGame);

			StartRound();
			StartCoroutine(NextMusicToGameFromDelay());
		}

		public void RestartGame(float timeDealy)
		{
			StartCoroutine(DealyRestartGame(timeDealy));
		}

		private IEnumerator DealyRestartGame(float time)
		{
			yield return new WaitForSeconds(time);
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		private IEnumerator NextMusicToGameFromDelay()
		{
			while (true)
			{
				yield return new WaitForSeconds(30f);
				AudioManager.Instance.NextMusic();
			}
		}
	}
}

