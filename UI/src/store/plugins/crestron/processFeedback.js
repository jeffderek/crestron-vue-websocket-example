// This function gets called any time a message is received from the websocket.
// This example uses a *very* simplistic protocol that would not actually be
// a good design for a real system.  It is highly recommended that you create
// a json schema that meets the needs for your specific system.  This particular
// example is just about transferring the strings back and forth between the
// Web UI and the processor, so it uses an intentionally simplistic api to avoid
// complication.
export default function processFeedback(store, message) {
    // Split the message with pipes
    let command = message.split('|');

    switch (command[0]) {
        case 'displays': {
            // Example message `displays|display_1|power_on`
            switch (command[2]) {
                case 'power_on': {
                    store.commit('displays/setPowerFeedback', {
                        id: command[1],
                        power: true,
                    });
                    break;
                }

                case 'power_off': {
                    store.commit('displays/setPowerFeedback', {
                        id: command[1],
                        power: false,
                    });
                    break;
                }
            }
            break;
        }

        case 'name': {
            // Example message `name|System Name`
            store.commit('setSystemName', command[1]);
            break;
        }

        case 'counter': {
            // Example message 'counter|42`
            store.commit('counter/setCounter', command[1]);
            break;
        }
    }
}
