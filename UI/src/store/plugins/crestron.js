// External Imports
import * as CrComLib from '@crestron/ch5-crcomlib/build_bundles/amd/cr-com-lib'; // The Crestron Communication Library
import debounce from 'lodash/debounce'; // A function for debouncing a changing variable
import eruda from 'eruda'; // On-screen console for debugging on a panel
import ReconnectingWebSocket from 'reconnecting-websocket'; // Websocket implementation that autoreconnects

// Project Imports
import processFeedback from './crestron/processFeedback'; // Process feedback from the processor into changing values in the store
import processMutation from './crestron/processMutation'; // Process store mutations into commands sent to the processor

// This function gets called by the default function with a different address depending on whether you
// are connecting from a touchpanel, a web browser hosted on your computer in dev mode, or a web
// page hosted on the processor.
function createWebSocket(store, address) {
    // Set the processor Address
    store.commit('connection/setProcessorAddress', address);

    // Close the existing socket if it exists
    store.commit('connection/closeSocket', null);

    // Create a new websocket pointing to the processor address
    let url = `ws://${address}:5000/app`;
    let socket = new ReconnectingWebSocket(url, [], {
        debug: false,
        startClosed: true,
    });

    // Store the websocket in Vuex so we can close it later if necessary
    store.commit('connection/setSocket', socket);

    // Any time a message is received, call the processFeedback function with the message data
    socket.onmessage = (msg) => {
        processFeedback(store, msg.data);
    };

    // Connect to the socket (since it was created with the startClosed option letting us finish setting up before connecting)
    socket.reconnect();
}

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

        // Check the CrComLib to see if the project is on a touchscreen
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

            // Subscribe to serial join 1.  CrComLib does not have access to the
            // IP Table, so the only way to get the IP address of the processor to connect
            // via websockets is to pass it from the processor.  The processor sends it's
            // ip address via serial join 1, so this subscription waits for that value to change and
            // sets up a connection to the address when it does.
            CrComLib.subscribeState('s', '1', (value) => {
                if (store.state.connection.processorAddress != value) {
                    // Create a new websocket
                    createWebSocket(store, value);
                }
            });
        } else {
            // If the project is not on a panel, check to see if it is hosted on a the local machine
            // in vue dev mode or if it is hosted on a processor.  The address of test processor is
            // set in the .env.development file.
            if (location.hostname == 'localhost') {
                createWebSocket(store, process.env.VUE_APP_LOCAL_PROCESSOR);
            } else {
                createWebSocket(store, location.hostname);
            }
        }

        // Subscribe to the Vuex mutations.  Every mutation that hits the Vuex store
        // will pass through the processMutation function, where it will be parsed and
        // Turned into join numbers.
        store.subscribe((mutation) => {
            processMutation(store, mutation);
        });
    };
}
