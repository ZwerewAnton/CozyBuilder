using UnityEngine;
using UnityEngine.UI;

namespace UI.Common.Dialog
{
    public class CancelAcceptCloseDialog : BaseDialog
    {
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button closeButton;

        private void OnEnable()
        {
            cancelButton.onClick.AddListener(Cancel);
            acceptButton.onClick.AddListener(Accept);
            closeButton.onClick.AddListener(Close);
        }

        private void OnDisable()
        {
            cancelButton.onClick.RemoveListener(Cancel);
            acceptButton.onClick.RemoveListener(Accept);
            closeButton.onClick.RemoveListener(Close);
        }
    }
}