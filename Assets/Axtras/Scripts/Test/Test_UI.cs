using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Test_UI : MonoBehaviour 
{
    [Header("Modals")]
    [SerializeField] private Type_Tutorial tutorialSO;
    [SerializeField] private GameObject modalPrefab;

    private void Start() {
        SpawnModal(tutorialSO);
    }
    
    public void FunctionA() {
    }
    public void FunctionB() {
    }

    public void SpawnModal(Type_Tutorial tutorialSO) {
        GameObject modalGO = Instantiate(modalPrefab);

        var modalCanvas = GameObject.Find("Canvii/Modals Canvas");
        modalGO.transform.SetParent(modalCanvas.transform);

        RectTransform modalRect = modalGO.GetComponent<RectTransform>();
        modalRect.anchoredPosition = new Vector2(0, 0);

        modalGO.transform.localScale = Vector3.zero;
        modalGO.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        var modalText = modalGO.transform.Find("Modal Text");
        var modalImages = modalGO.transform.Find("Modal Images");
        var modalButtons = modalGO.transform.Find("Modal Buttons");

        var titleText = modalText.transform.Find("Title Text").GetComponent<TMP_Text>();
        titleText.text = tutorialSO.titleStr;
        var contentText = modalText.transform.Find("Content Text").GetComponent<TMP_Text>();
        contentText.text = tutorialSO.contentStr;

        var imageImage = modalImages.transform.Find("Image Image").GetComponent<Image>();
        imageImage.sprite = tutorialSO.GetSprite();
        var leftButton = modalImages.transform.Find("Left Button").GetComponent<Button>();
        leftButton.onClick.AddListener(() => {
            imageImage.sprite = tutorialSO.GetSprite(-1);
        });
        var rightButton = modalImages.transform.Find("Right Button").GetComponent<Button>();
        rightButton.onClick.AddListener(() => {
            imageImage.sprite = tutorialSO.GetSprite(+1);
        });

        var yesButton = modalButtons.transform.Find("Yes Button").GetComponent<Button>();
        yesButton.onClick.AddListener(() => {
            tutorialSO.onYesClicked?.Invoke();
            modalGO.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack).OnComplete(() => Destroy(modalGO));
        });
        var noButton = modalButtons.transform.Find("No Button").GetComponent<Button>();
        noButton.onClick.AddListener(() => {
            tutorialSO.onNoClicked?.Invoke();
            modalGO.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack).OnComplete(() => Destroy(modalGO));
        });

        var yesButtonText = yesButton.transform.Find("Yes Button Text").GetComponent<TMP_Text>();
        yesButtonText.text = tutorialSO.yesButtonStr;
        var noButtonText = noButton.transform.Find("No Button Text").GetComponent<TMP_Text>();
        noButtonText.text = tutorialSO.noButtonStr;
    }
}