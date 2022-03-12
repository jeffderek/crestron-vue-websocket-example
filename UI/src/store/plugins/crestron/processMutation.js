// This function listens to all mutations that happen in the system, and
// when certain mutations are committed it sends strings over the websocket
// to the processor for the processor to react.
export default function processMutation(store, mutation) {
    // Logging the mutation done here just to show an example of how eruda can be
    // used to debug issues on the panel.
    console.log(mutation);

    // Storing the socket as a local variable to make it easier to type below
    let socket = store.state.connection.socket;

    // Switch/Case on the mutation type lets us handle each mutation individually
    // This can easily get out of hand for a system with a lot of mutations or
    // commands that need to be sent back to the system.  Consider carefully how
    // you structure your code to minimize code bloat in this function.
    switch (mutation.type) {
        case 'counter/incrementCounter': {
            socket?.send('counter|increment');
            break;
        }
        case 'counter/decrementCounter': {
            socket?.send('counter|decrement');
            break;
        }
        case 'displays/setPower': {
            // The mutation payload is a single object that uses destructuring
            // to include however many properties you need.  The payload for the
            // setPower function includes an id and a power boolean.
            socket?.send(
                `displays|${mutation.payload.id}|${
                    mutation.payload.power ? 'power_on' : 'power_off'
                }`
            );
            break;
        }
    }
}
