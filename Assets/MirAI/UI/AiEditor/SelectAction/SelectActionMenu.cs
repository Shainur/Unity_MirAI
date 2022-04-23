﻿using Assets.MirAI.Utils.Disposables;

namespace Assets.MirAI.UI.AiEditor.SelectAction {

    public class SelectActionMenu : MenuController {

        public readonly CompositeDisposable _trash = new CompositeDisposable();

        public override void Start() {
            base.Start();
        }

        private void OnDestroy() {
            _trash.Dispose();
        }
    }
}