<template>
  <div>
    {{headerText}}
    {{header}}
    {{items.length}}
    <ul>
      <li v-for="todo in items" v-bind:key="todo.text">
        <!--
        We have a slot for each todo, passing it the
        `todo` object as a slot prop.
        -->
        <slot name="tpl" v-bind="todo">
          <!-- Fallback content -->
          Fallback: {{ todo.text }}
        </slot>
      </li>
    </ul>
  </div>
</template>

<script lang="ts">
import VueRouter, { RouteConfig, Route } from 'vue-router'
import Vue from 'vue'
import { Component, Prop } from 'vue-property-decorator'

@Component({
})
export default class ListBox extends Vue {
  @Prop() header!: string
  @Prop() items!: Array<any>

  public get headerText (): string {
    return this.header
  }
}

</script>
