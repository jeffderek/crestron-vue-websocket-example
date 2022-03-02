<template>
    <div id="counter">
        <div id="value">{{ $store.state.counter }}</div>
        <div id="buttons">
            <button class="btn-default" @click="decrementCounter">-</button>
            <button class="btn-default" @click="incrementCounter">+</button>
        </div>
    </div>
</template>

<script>
import { mapMutations } from 'vuex';

export default {
    methods: {
        ...mapMutations(['incrementCounter', 'decrementCounter']),
    },
};
</script>

<style lang="scss">
#counter {
    height: 90%;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;

    #value {
        font-size: 8rem;
    }

    .btn-default {
        height: 60px;
        width: 120px;
        font-size: 2rem;
    }
}
</style>
