<template>
  <b-overlay :show="visible" no-wrap></b-overlay>
</template>

<script lang="ts">
import { Prop, Component } from 'vue-property-decorator'
import Vue from 'vue'

@Component({})
export default class SimpleSpinnedComponent extends Vue {
  @Prop() visible!: boolean
}
</script>
