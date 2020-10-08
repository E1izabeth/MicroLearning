<template>
  <b-modal id="modal-topic-contents" size="xl" centered :title="topic.Title" no-close-on-backdrop no-close-on-esc :hide-header-close="loadingData" @hidden="onContentsHidden">
    <SimpleSpinnerComponent :visible="loadingData" />

    <ListView v-bind:itemsSource="topicContents" @itemClick="onContentItemClick">
      <template v-slot:title="item">{{item.res.Title}}</template>
      <template v-slot:titleStatus="item">
        <b-badge v-if="item.res.IsValidated" variant="success" pill >Approved</b-badge>
        <b-badge v-if="!item.res.IsValidated" variant="secondary" pill >Awaiting</b-badge>
      </template>
      <template v-slot:content="item"><a :href="item.res.Url" target="blank">{{item.res.Url}}</a></template>
      <template v-slot:tags="item">
        <em v-if="!item.previewExpanded">{{item.preview}}</em>
        <em v-if="item.previewExpanded"><p v-for="para in item.content" v-bind:key="para.index">{{para.text}}</p></em>
      </template>
      <template v-slot:contentStatus="item">
        <b-button v-if="item.info.IsLearned" variant="success" class="mb-0" size="sm" @click="markUnlearned(item)">Learned</b-button>
        <b-button v-if="!item.info.IsLearned" variant="outline-primary" class="mb-0" size="sm" @click="markLearned(item)">Mark learned</b-button>
        <b-button class="mb-0 mt-0" variant="link" @click="item.toggle()">
          <b-icon v-if="!item.previewExpanded" icon="chevron-double-down"></b-icon>
          <b-icon v-if="item.previewExpanded" icon="chevron-compact-up"></b-icon>
        </b-button>
      </template>
      <template v-slot:tagsStatus="item">
        <b-badge v-if="item.info.IsDelivered" variant="light">Delivered</b-badge>
      </template>
    </ListView>

    <template v-slot:modal-footer="{ ok }">
      <b-button :disabled="loadingData" variant="success" @click="ok()" ref="btnTopicOk" >OK</b-button>
    </template>
  </b-modal>
</template>

<script lang="ts">
import Component from 'vue-class-component'
import VueRouter, { RouteConfig, Route } from 'vue-router'
import ListView, { ListViewItem } from '../controls/ListView.vue'
import SimpleSpinnerComponent from '../components/SimpleSpinnerComponent.vue'
import Vue from 'vue'
import api from '../api'
import * as x from '../../xml/my-json-types'

class ContentParagraph {
  // eslint-disable-next-line
  public constructor (
    public index: number,
    public text: string) {
  }
}

class ContentListItem extends ListViewItem {
  info: x.ContentPartInfoType
  res: x.ResourceInfoType
  content: Array<ContentParagraph>
  preview: string
  previewExpanded: boolean

  public constructor (info: x.ContentPartInfoType, res: x.ResourceInfoType) {
    super()
    this.info = info
    this.res = res
    this.content = info.Text.split('\n').map((s, n) => new ContentParagraph(n, s))
    this.preview = info.Text.substring(0, Math.min(info.Text.length, 200)) + '...'
    this.previewExpanded = false
  }

  public toggle = async () => {
    this.previewExpanded = !this.previewExpanded
  }

  public key (): any {
    return this.info.Id
  }
}

@Component({
  components: { ListView, SimpleSpinnerComponent }
})
export default class TopicProperties extends Vue {
  topic = { } as x.TopicInfoType
  topicContents = new Array<ContentListItem>()

  loadingData = false

  async created () {
    try {
      this.loadingData = true

      this.topic = await api.getTopicById(Number.parseInt(this.$router.currentRoute.params.topicId))

      const resources: { [email: string]: x.ResourceInfoType } = { }
      const test = await api.getContentPartsByTopic(this.topic.Id, 0, 0)
      const contents = await api.getContentPartsByTopic(this.topic.Id, 0, test.Count)

      this.topicContents = new Array<ContentListItem>()
      if (contents.Items) {
        for (const item of contents.Items) {
          let res = resources[item.ResourceId]
          if (!res) {
            resources[item.ResourceId] = res = await api.getResourceById(item.ResourceId)
          }
          this.topicContents.push(new ContentListItem(item, res))
        }
      }
    } catch (err) {
      await this.$router.push('/app/topics')
      await this.$nextTick()
      this.$bvToast.toast(err.message, { title: 'Error opening topic contents', variant: 'warning', solid: true })
    } finally {
      this.loadingData = false
    }
  }

  async mounted () {
    this.$bvModal.show('modal-topic-contents')
  }

  async onContentsHidden () {
    this.$emit('reloadTopics')
    await this.$nextTick()
    this.$router.push('/app/topics')
  }

  async onContentItemClick () {
    // alert('')
  }

  async markLearned (item: ContentListItem) {
    this.loadingData = true
    try {
      api.markContentPartLearned(this.topic.Id, item.info.Id)
      item.info.IsLearned = true
    } catch (err) {
      this.$bvToast.toast(err.message, { title: 'Error marking content part as learned', variant: 'warning', solid: true })
    } finally {
      this.loadingData = false
    }
  }

  async markUnlearned (item: ContentListItem) {
    this.loadingData = true
    try {
      api.unmarkContentPartLearned(this.topic.Id, item.info.Id)
      item.info.IsLearned = false
    } catch (err) {
      this.$bvToast.toast(err.message, { title: 'Error unmarking content part as learned', variant: 'warning', solid: true })
    } finally {
      this.loadingData = false
    }
  }
}
</script>
