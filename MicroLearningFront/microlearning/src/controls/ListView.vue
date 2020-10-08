<template>
  <div>
    <b-list-group id="list">
      <b-list-group-item class="flex-column align-items-start" v-for="item in itemsSource" v-bind:key="item.key()" @click="onItemClick($event, item)">
        <b-container class="bv-example-row">
          <b-row>
            <b-col @click="onTitleClick(item)"><h5><slot name="title" v-bind="item" /></h5></b-col>
            <b-col><slot name="titleStatus" v-bind="item" /></b-col>
          </b-row>
          <b-row>
            <b-col><slot name="content" v-bind="item"/></b-col>
            <b-col><slot name="contentStatus" v-bind="item"/></b-col>
          </b-row>
          <b-row>
            <b-col><small><slot name="tags" v-bind="item"/></small></b-col>
            <b-col><small><slot name="tagsStatus" v-bind="item"/></small></b-col>
          </b-row>
        </b-container>
      </b-list-group-item>
    </b-list-group>
  </div>
</template>

<script lang="ts">
import { Component, Prop } from 'vue-property-decorator'
import VueRouter, { RouteConfig, Route } from 'vue-router'
import Vue from 'vue'

export class ListViewItem {
  private static _id = 0

  protected _id: number

  public constructor () {
    this._id = ListViewItem._id++
  }

  public key (): any {
    return this._id
  }
}

@Component({
})
export default class ListView extends Vue {
  @Prop() itemsSource!: Array<ListViewItem>

  onTitleClick (item: ListViewItem) {
    try {
      this.$emit('titleClick', item)
    } catch (er) {
      console.error('onTitleClick handler failed with ' + er)
    }
  }

  onItemClick (event: MouseEvent, item: ListViewItem) {
    try {
      if (event.target instanceof HTMLDivElement) {
        this.$emit('itemClick', item)
      }
    } catch (er) {
      console.error('onItemClick handler failed with ' + er)
    }
  }
}
</script>

<style lang="scss">
#table {

  .input-group-text {
    padding: 0 .5em 0 .5em;

    .fa {
      font-size: 12px;
    }
  }
}

</style>
