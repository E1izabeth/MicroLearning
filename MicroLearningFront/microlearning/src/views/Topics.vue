<template>
  <b-container class="bv-example-row" id="table">
    <SimpleSpinnerComponent :visible="loadingData" />

    <router-view @reloadTopics="reloadTopics"></router-view>

    <b-row class="justify-content-md-center">
      <b-col>

        <b-alert v-if="!this.isProfileActivated" show variant="danger">Please, activate your profile first!</b-alert>

        <b-input-group size="md" class="mt-3 mb-3">
          <b-form-input v-model="keyword" placeholder="Search" type="text" @input="onKeywordChange" />
          <b-input-group-text slot="append" v-if="keyword.length > 0">
            <b-button  variant="link" size="sm" @click="resetKeyword" class="p-0"><b>x</b></b-button>
          </b-input-group-text>
        </b-input-group>

        <ListView v-bind:itemsSource="currentItems" @itemClick="onTopicItemClick">
          <template v-slot:title="item">
            {{item.info.Title}}
            <!--<b-button class="mb-1 mt-0" variant="link" @click="onTopicTitleClick(item)"><small><b-icon icon="gear"></b-icon></small></b-button>-->
          </template>
          <template v-slot:titleStatus="item">
            <b-button v-if="item.info.IsActive" variant="success" class="mb-0" size="sm" @click="doDeactivateTopic(item)">Deactivate</b-button>
            <b-button v-if="!item.info.IsActive" variant="outline-primary" class="mb-0" size="sm" @click="doActivateTopic(item)">Activate</b-button>
          </template>
          <template v-slot:tagsStatus="item"><b-badge v-bind:variant="(item.finished ? 'light' : 'primary')" pill>{{item.info.LearnedContentParts}}/{{item.info.TotalContentParts}}</b-badge></template>
          <template v-slot:tags="item"><em>{{item.tags}}</em></template>
        </ListView>

        <b-card v-if="items.length == 0 && !loadingData" title="Subscriptions not found" sub-title="0 results">
          <b-card-text class="text-center">
            Sorry, there is nothing to display
          </b-card-text>
        </b-card>

      </b-col>
    </b-row>
  </b-container>
</template>

<script lang="ts">
import Component from 'vue-class-component'
import VueRouter, { RouteConfig, Route } from 'vue-router'
import ListView, { ListViewItem } from '../controls/ListView.vue'
import SimpleSpinnerComponent from '../components/SimpleSpinnerComponent.vue'
import Vue from 'vue'
import api from '../api'
import * as x from '../../xml/my-json-types'

class TopicListItem extends ListViewItem {
  public info: x.TopicInfoType
  public tags: string
  public finished: boolean

  public constructor (info: x.TopicInfoType) {
    super()
    this.info = info
    this.tags = info.AssociationTags ? info.AssociationTags.map(t => t.Word).join(', ') : '---'
    this.finished = info.TotalContentParts && info.LearnedContentParts ? info?.TotalContentParts <= info.LearnedContentParts : true
  }

  public key (): any { return this.info.Id }
}

@Component({
  components: { ListView, SimpleSpinnerComponent }
})
export default class Topics extends Vue {
  tmp = ''

  keyword = ''
  items = new Array<TopicListItem>()
  currentItems = new Array<TopicListItem>()
  loadingData = false

  isProfileActivated = false

  async created () {
    this.isProfileActivated = await api.currentProfile().then(t => t.IsActivated)
    await this.loadTopics()
  }

  async loadTopics () {
    this.loadingData = true
    try {
      const topics = await api.getTopics()
      if (topics.Items) {
        this.items = new Array<TopicListItem>()
        topics.Items.forEach(t => this.items.push(new TopicListItem(t)))
        this.currentItems = new Array<TopicListItem>()
        this.items.forEach(t => this.currentItems.push(t))
      }
    } catch (err) {
      this.$bvToast.toast(err.message, { title: 'Error loading data', variant: 'warning', solid: true })
    } finally {
      this.loadingData = false
    }
  }

  keywordLen = 0

  async onKeywordChange (e: KeyboardEvent|null) {
    if (this.keywordLen < this.keyword.length) {
      this.currentItems = this.currentItems.filter(item => item.info.Title.toLowerCase().indexOf(this.keyword.toLowerCase()) > -1)
    } else if (this.keywordLen > this.keyword.length) {
      this.currentItems = this.items.filter(item => item.info.Title.toLowerCase().indexOf(this.keyword.toLowerCase()) > -1)
    }
    this.keywordLen = this.keyword.length
  }

  async resetKeyword () {
    this.keyword = ''
    this.currentItems = new Array<TopicListItem>()
    this.items.forEach(t => this.currentItems.push(t))
  }

  async reloadTopics () {
    await this.loadTopics()
    await this.onKeywordChange(null)
  }

  // async onTopicTitleClick (item: TopicListItem) {
  //   this.$router.push(`/app/topics/${item.info.Id}/properties`)
  // }

  async onTopicItemClick (item: TopicListItem) {
    this.$router.push(`/app/topics/${item.info.Id}`)
  }

  async doActivateTopic (item: TopicListItem) {
    this.$router.push(`/app/topics/${item.info.Id}/properties`)
  }

  async doDeactivateTopic (item: TopicListItem) {
    this.loadingData = true
    try {
      api.deactivateTopicLearningProgress(item.info.Id)
      item.info.IsActive = false
    } catch (err) {
      this.$bvToast.toast(err.message, { title: 'Error deactivating topic', variant: 'warning', solid: true })
    } finally {
      this.loadingData = false
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
