using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class UIInteraction : MonoBehaviour {
	public static UIInteraction instance;

	[Header("MainMenu UI")]
	public CanvasGroup m_MainMenuPanel;

	[Header("Level UI")]
	public CanvasGroup m_LevelPanel;

	[Header("Quit UI")]
	public CanvasGroup m_QuitPanel;

	public void OpenLevel()
	{
		m_LevelPanel.alpha = 0;
		m_LevelPanel.gameObject.SetActive (true);

        m_LevelPanel.DOFade(1, 0.5f).OnComplete(() =>
        {

        });
	}
}