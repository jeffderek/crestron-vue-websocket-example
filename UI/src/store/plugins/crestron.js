// External Imports
import * as CrComLib from '@crestron/ch5-crcomlib/build_bundles/amd/cr-com-lib'; // The Crestron Communication Library
import debounce from 'lodash/debounce'; // A function for debouncing a changing variable
import eruda from 'eruda'; // On-screen console for debugging on a panel

// Project Imports
import processFeedback from './crestron/processFeedback'; // Process Feedback Joins into changes of the store
import processMutation from './crestron/processMutation';

// This function is run by Vuex when it creates the plugin.  The vuex store is passed
// in to the function so that the plugin can access the store.
export default function createCrestronPlugin() {
    return (store) => {
        // Eruda is an on-screen javascript console for troubleshooting when loaded to the panel.
        // This line uses an environment variable to only initialize eruda if you have compiled
        // the project in development mode, so that you can compile for production and not leave
        // the console on screen.
        if (process.env.NODE_ENV === 'development') {
            eruda.init();
        }

        // Check the CrComLib to make sure the project is on a touchscreen
        if (CrComLib.isCrestronTouchscreen()) {
            // These make some of the CrComLib functions accessible to the global window namespace
            // You can leave them in place and pretty much ignore them.
            window.bridgeReceiveIntegerFromNative =
                CrComLib.bridgeReceiveIntegerFromNative;
            window.bridgeReceiveBooleanFromNative =
                CrComLib.bridgeReceiveBooleanFromNative;
            window.bridgeReceiveStringFromNative =
                CrComLib.bridgeReceiveStringFromNative;
            window.bridgeReceiveObjectFromNative =
                CrComLib.bridgeReceiveObjectFromNative;

            // A function to debounce change in online state to prevent multiple triggers
            // The actual online state bool flickers a bit on transition
            let changeOnlineState = debounce(function (state) {
                console.log(
                    `Debounced CrestronPanel ${state ? 'Online' : 'Offline'}`
                );
            }, 500);

            // Subscribe to the Control system online state feedback
            // and send it to the changeOnlineState function which debounces it
            // No action is taken based on the online state in this project, this is
            // just an example of subscribing to a named join and debouncing the variable
            // The console logs are left in place so you can see how it works in eruda.
            CrComLib.subscribeState(
                'b',
                'Csig.All_Control_Systems_Online_fb',
                (state) => {
                    console.log(
                        `Raw CrestronPanel ${state ? 'Online' : 'Offline'}`
                    );
                    changeOnlineState(state);
                }
            );

            // Subscribe to the Vuex mutations.  Every mutation that hits the Vuex store
            // will pass through the processMutation function, where it will be parsed and
            // Turned into join numbers.
            store.subscribe((mutation, state) => {
                processMutation(store, mutation);
            });

            // Subscribe to CrComLib feedback.  All joins on the panel will be subscribed to
            // and turned into store mutations
            processFeedback(store);
        }
    };
}
