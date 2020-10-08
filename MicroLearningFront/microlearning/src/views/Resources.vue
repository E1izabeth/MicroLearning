<template>
  <b-container class="bv-example-row" id="table">
    <SimpleSpinnerComponent :visible="loadingData" />

    <b-modal id="modal-create-resource" centered title="Register new resource" @show="resResetModal" @hidden="resResetModal" @ok="resHandleOk"
             no-close-on-backdrop no-close-on-esc :hide-header-close="resFormWaiting">
      <DataSumbitionComponent ref="resWaiter" />

      <b-form @submit.stop.prevent="resHandleSubmit">
        <b-form-group  label="Title:" >
          <b-form-input v-model="resFormTitle" type="text" placeholder="Title" required></b-form-input>
        </b-form-group>
        <b-form-group label="Url:">
          <b-form-input v-model="resFormUrl" type="text" placeholder="Url" required></b-form-input>
        </b-form-group>
        <b-form-group label="Tags:">
          <b-form-input v-model="resFormTags" type="text" placeholder="Tags" required></b-form-input>
        </b-form-group>
      </b-form>

      <template v-slot:modal-footer="{ ok, cancel }">
        <b-button :disabled="resFormWaiting" variant="outline-primary" @click="resSuggestResource" >Suggest tags and title</b-button>
        <b-button :disabled="resFormWaiting" variant="success" @click="ok()" ref="btnResOk" >OK</b-button>
        <b-button :disabled="resFormWaiting" variant="outline-primary" @click="cancel()" >Cancel</b-button>
      </template>
    </b-modal>
    <b-modal id="modal-create-topic" centered title="Subscribe for a topic" @show="topicSetupModel" @hidden="topicResetModal" @ok="topicHandleOk"
             no-close-on-backdrop no-close-on-esc :hide-header-close="topicFormWaiting">
      <DataSumbitionComponent ref="topicWaiter" />

      <b-form @submit.stop.prevent="topicHandleSubmit">
        <b-form-group  label="Title:" >
          <b-form-input v-model="topicFormTitle" type="text" placeholder="Title" required></b-form-input>
        </b-form-group>
        <b-form-group label="Tags:">
          <b-form-input v-model="topicFormTags" type="text" placeholder="Tags" required></b-form-input>
        </b-form-group>
      </b-form>

      <template v-slot:modal-footer="{ ok, cancel }">
        <b-button :disabled="topicFormWaiting" variant="success" @click="ok()" ref="btnTopicOk" >OK</b-button>
        <b-button :disabled="topicFormWaiting" variant="outline-primary" @click="cancel()" >Cancel</b-button>
      </template>
    </b-modal>

    <b-row class="justify-content-md-center">
      <b-col cols="1">
        <b-input-group size="md" class="mt-5 mb-3">
          <b-button variant="outline-primary" class="mt-5" v-b-modal.modal-create-resource><strong>+</strong></b-button>
        </b-input-group>
      </b-col>
      <b-col cols="10">

        <b-alert v-if="!this.isProfileActivated" show variant="danger">Please, activate your profile first!</b-alert>

        <b-input-group size="md" class="mt-3 mb-3">
          <b-form-input v-model="keyword" placeholder="Search" type="text" ref="txtSearchBox" @keydown="onSearchBoxKeyDown" />
          <b-input-group-text slot="append">
            <b-button v-if="filterApplied" variant="link" size="sm" @click="doResetSearch()" class="mr-sm-2"><b>x</b></b-button>
            <b-button v-bind:disabled="keyword.length < 1" variant="link" size="sm" @click="doSearch()" class="p-0 mr-sm-2"><b>Search</b></b-button>
          </b-input-group-text>
        </b-input-group>

        <ListView v-bind:itemsSource="items">
          <template v-slot:title="item">{{item.info.Title}}</template>
          <template v-slot:titleStatus="item">
            <b-badge v-if="item.info.IsValidated" variant="success" pill >Approved</b-badge>
            <b-badge v-if="!item.info.IsValidated" variant="secondary" pill >Awaiting</b-badge>
          </template>
          <template v-slot:content="item"><a :href="item.info.Url" target="blank">{{item.info.Url}}</a></template>
          <template v-slot:contentStatus="item">
            <b-button v-if="(!item.info.IsMyResource && !item.info.IsValidated)" @click="item.onClick()"
                      variant="success" class="mb-0" size="sm">Approve</b-button>
          </template>
          <template v-slot:tags="item"><em>{{item.tags}}</em></template>
        </ListView>

         <b-card v-if="items.length == 0 && !loadingData" title="Nothing found" sub-title="0 results">
          <b-card-text class="text-center">
            May be we have nothing about your topic, or your can try different keywords
          </b-card-text>
        </b-card>

      </b-col>
      <b-col cols="1">
        <b-input-group size="md" class="mt-5 mb-3" v-if="filterApplied && items.length > 0 ">
          <b-button variant="success" class="mt-5" v-b-modal.modal-create-topic >Subscribe</b-button>
        </b-input-group>
      </b-col>
    </b-row>
  </b-container>
</template>

<script lang="ts">
import Component from 'vue-class-component'
import VueRouter, { RouteConfig, Route } from 'vue-router'
import DataSumbitionComponent from '../components/DataSumbitionComponent.vue'
import SimpleSpinnerComponent from '../components/SimpleSpinnerComponent.vue'
import ListView, { ListViewItem } from '../controls/ListView.vue'
import Vue from 'vue'
import api from '../api'
import * as x from '../../xml/my-json-types'

class ResourceListItem extends ListViewItem {
  public info: x.ResourceInfoType
  public tags: string

  public constructor (info: x.ResourceInfoType) {
    super()
    this.info = info
    this.tags = info.AssociationTags === undefined || info.AssociationTags === undefined ? '---' : info.AssociationTags.map(t => t.Word)?.join(', ')
  }

  public onClick = async () => {
    await api.markResourceValidated(this.info.Id)
    this.info = await api.getResourceById(this.info.Id)
  }

  public key (): any { return this.info.Id }
}

function p<T> (arg: Partial<T>): T {
  return arg as T
}

@Component({
  components: { ListView, DataSumbitionComponent, SimpleSpinnerComponent }
})
export default class Resources extends Vue {
  tmp = ''

  keyword = ''
  isProfileActivated = false

  public loadingData = true
  public items = new Array<ResourceListItem>()
  public filterApplied = false

  async created () {
    this.isProfileActivated = await api.currentProfile().then(t => t.IsActivated)
  }

  get resWaiter (): DataSumbitionComponent {
    return (this.$refs.resWaiter as DataSumbitionComponent)
  }

  get topicWaiter (): DataSumbitionComponent {
    return (this.$refs.topicWaiter as DataSumbitionComponent)
  }

  async mounted () {
    this.doResetSearch()
  }

  async doResetSearch () {
    this.loadingData = true
    this.filterApplied = false
    try {
      const resources = await api.getResources()
      this.items = new Array<ResourceListItem>()
      if (resources.Items) {
        resources.Items.forEach(r => this.items.push(new ResourceListItem(r)))
      }

      const inputBox = (this.$refs.txtSearchBox as HTMLInputElement)
      if (inputBox) {
        inputBox.selectionStart = 0
        inputBox.selectionEnd = inputBox.value.length
        inputBox.focus()
      }
    } catch (err) {
      this.$bvToast.toast(err.message, { title: 'Error retrieving resources', variant: 'warning', solid: true })
    } finally {
      this.loadingData = false
    }
  }

  async doSearch () {
    this.loadingData = true
    try {
      const test = await api.filterResourcesRange({ tags: this.keyword.split(',') }, 0, 0)
      const resources = await api.filterResourcesRange({ tags: this.keyword.split(',') }, 0, test.Count)

      this.items = new Array<ResourceListItem>()
      if (resources.Items) {
        resources.Items.forEach(r => this.items.push(new ResourceListItem(r)))
      }

      this.filterApplied = true
    } catch (err) {
      this.$bvToast.toast(err.message, { title: 'Error searching by tags', variant: 'warning', solid: true })
    } finally {
      this.loadingData = false
    }
  }

  async onSearchBoxKeyDown (e: KeyboardEvent) {
    if (e.keyCode === 13) {
      this.doSearch()
    } else if (e.keyCode === 27) {
      this.doResetSearch()
    }
  }

  resFormTitle = ''
  resFormUrl = ''
  resFormTags = ''
  resFormWaiting = false

  async resResetModal () {
    this.resFormTitle = ''
    this.resFormUrl = ''
    this.resFormTags = ''
  }

  async resHandleOk (bvModalEvt: Event) {
    bvModalEvt.preventDefault() // Prevent modal from closing
    this.resHandleSubmit()
  }

  async resHandleSubmit () {
    this.resFormWaiting = true
    await this.resWaiter.doShow({
      confirmationCallback: async () => {
        try {
          if (this.resFormTitle.trim().length === 0) {
            throw new Error('Resource title should not be empty')
          }
          const resource = await api.createResource(this.resFormTitle, this.resFormUrl, this.resFormTags.split(',').map(s => s.trim()).filter(s => s.length > 0))
          this.items.splice(0, 0, new ResourceListItem(resource))
          this.$nextTick(() => this.$bvModal.hide('modal-create-resource'))
        } catch (err) {
          this.$bvToast.toast(err.message, { title: 'Error creating resource', variant: 'warning', solid: true })
        } finally {
          this.resWaiter.doFinish()
        }
      },
      cancelCallback: async () => {
        (this.$refs.btnResOk as HTMLElement).focus()
      }
    })
    this.resFormWaiting = false
  }

  async resSuggestResource () {
    this.resFormWaiting = true
    try {
      this.resWaiter.doShow()
      const spec = await api.suggestResource(this.resFormTitle, this.resFormUrl, this.resFormTags.split(',').map(s => s.trim()).filter(s => s.length > 0))

      this.resFormTitle = spec.ResourceTitle
      this.resFormUrl = spec.ResourceUrl
      this.resFormTags = spec.AssociationTags.map(t => t.Word).join(', ')
    } catch (err) {
      this.$bvToast.toast(err.message, { title: 'Error suggesting resource', variant: 'warning', solid: true })
    } finally {
      this.resWaiter.doFinish()
    }
    this.resFormWaiting = false
  }

  topicFormTitle = ''
  topicFormTags = ''
  topicFormWaiting = false

  async topicSetupModel () {
    this.topicFormTags = this.keyword
    this.topicFormTitle = ''
  }

  async topicResetModal () {
    this.topicFormTitle = ''
    this.topicFormTags = ''
  }

  async topicHandleOk (bvModalEvt: Event) {
    bvModalEvt.preventDefault() // Prevent modal from closing
    this.topicHandleSubmit()
  }

  async topicHandleSubmit () {
    this.topicFormWaiting = true
    await this.topicWaiter.doShow({
      confirmationCallback: async () => {
        try {
          if (this.topicFormTitle.trim().length === 0) {
            throw new Error('Topic title should not be empty')
          }
          const topic = await api.createTopic(this.topicFormTitle, this.topicFormTags.split(',').map(s => s.trim()).filter(s => s.length > 0))
          this.$nextTick(() => this.$bvModal.hide('modal-create-topic'))
        } catch (err) {
          this.$bvToast.toast(err.message, { title: 'Error creating topic', variant: 'warning', solid: true })
        } finally {
          this.topicWaiter.doFinish()
        }
      },
      cancelCallback: async () => {
        (this.$refs.btnTopicOk as HTMLElement).focus()
      }
    })
    this.topicFormWaiting = false
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
