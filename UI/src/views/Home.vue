<template>
    <div id="counter">
        <h1>{{ $store.state.systemName }}</h1>
        <div id="value">{{ $store.state.counter.value }}</div>
        <div id="buttons">
            <button class="btn-default primary" @click="decrementCounter">
                -
            </button>
            <button class="btn-default primary" @click="incrementCounter">
                +
            </button>
        </div>
    </div>
</template>

<script>
import { mapMutations } from 'vuex';

export default {
    methods: {
        ...mapMutations('counter', ['incrementCounter', 'decrementCounter']),
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
        font-size: 40px;
    }
}
</style>
