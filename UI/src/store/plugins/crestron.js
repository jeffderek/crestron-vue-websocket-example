import * as CrComLib from '@crestron/ch5-crcomlib/build_bundles/amd/cr-com-lib';
import processFeedback from './crestron/processFeedback';
import processMutation from './crestron/processMutation';
import eruda from 'eruda';
import debounce from 'lodash/debounce';

// Eruda is an on-screen javascript console for troubleshooting when loaded to the panel
if (process.env.NODE_ENV === 'development') {
    eruda.init();
}

export default function createCrestronPlugin() {
    return (store) => {
        if (CrComLib.isCrestronTouchscreen()) {
            window.bridgeReceiveIntegerFromNative =
                CrComLib.bridgeReceiveIntegerFromNative;
            window.bridgeReceiveBooleanFromNative =
                CrComLib.bridgeReceiveBooleanFromNative;
            window.bridgeReceiveStringFromNative =
                CrComLib.bridgeReceiveStringFromNative;
            window.bridgeReceiveObjectFromNative =
                CrComLib.bridgeReceiveObjectFromNative;

            // Debounces change in online state to prevent multiple triggers
            // The actual online state bool flickers a bit on transition
            let changeOnlineState = debounce(function (state) {
                if (state) {
                    console.log('CrestronPanel Online');
                } else {
                    console.log('CrestronPanel Offline');
                }
            }, 500);

            // Subscribe to the Control system online state feedback
            // and send it to the changeOnlineState function which debounces it
            CrComLib.subscribeState(
                'b',
                'Csig.All_Control_Systems_Online_fb',
                (state) => {
                    changeOnlineState(state);
                }
            );

            store.subscribe((mutation, state) => {
                processMutation(store, mutation);
            });

            processFeedback(store);
        }
    };
}
