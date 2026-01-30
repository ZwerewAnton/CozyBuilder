using System;
using UnityEngine;

namespace UI.Common.Dialog
{
    public class BaseDialog : MonoBehaviour
    {
        public event Action<DialogResult> Completed;

        protected void Accept()
        {
            Finish(DialogResult.Accept);
        }

        protected void Cancel()
        {
            Finish(DialogResult.Cancel);
        }

        protected void Close()
        {
            Finish(DialogResult.Close);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        protected void Finish(DialogResult result)
        {
            Completed?.Invoke(result);
            Hide();
        }
    }
}