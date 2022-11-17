using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using flanne;
using flanne.Core;
using System.Collections;

namespace BetterUI
{

public class CustomInputs
{
	protected static GameController gameController = null;

	[HarmonyPostfix]
	[HarmonyPatch(typeof(PauseState), "Enter")]
	private static void PauseStateEnterPostPatch()
	{
		PausePatchManager.Instance.enablePause();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(PauseState), "OnResume")]
	private static void PauseStateOnResumePostPatch()
	{
		PausePatchManager.Instance.isPaused = false;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(GameController), "Start")]
	private static void GameControllerStartPostPatch(GameController __instance)
	{
		gameController = __instance;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(PlayerController), "Update")]
	private static void PlayerControllerUpdatePostPatch()
	{
		if (gameController != null)
		{
			var keyboard = Keyboard.current;

			if (keyboard == null)
				return; // No keyboard connected.

			if (keyboard.escapeKey.wasPressedThisFrame)
			{
				if (PausePatchManager.Instance.isPaused)
				{
					if (gameController.CurrentState is PauseState)
						gameController.pauseResumeButton.onClick.Invoke();
					else if (gameController.CurrentState is OptionsState)
						gameController.optionsBackButton.onClick.Invoke();
					else if (gameController.CurrentState is SynergyUIState)
						gameController.synergiesUIBackButton.onClick.Invoke();
				}
			}
		}
	}
}

public class PausePatchManager : MonoBehaviour
{
	private static PausePatchManager _instance;

	public bool isPaused = false;
	private float unpauseDelay = .25f;

	public static PausePatchManager Instance
	{
		get
		{
			if(_instance == null)
			{
				var gameObj = new GameObject();
				_instance = gameObj.AddComponent<PausePatchManager>();
			}

			return _instance;
		}
	}

	void Awake()
	{
		_instance = this;
	}

	public void enablePause()
	{
		if (!isPaused)
			base.StartCoroutine(this.puseEnableCR());
	}

	private IEnumerator puseEnableCR()
	{
		yield return new WaitForSecondsRealtime(unpauseDelay);
		isPaused = true;
		yield break;
	}
}
}
