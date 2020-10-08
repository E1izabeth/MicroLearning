<template>
  <b-overlay :show="busy" no-wrap @shown="onShown" @hidden="onHidden">
    <template v-slot:overlay>
      <div v-if="processing" class="text-center p-4 bg-primary text-light rounded">
        <b-icon icon="cloud-upload" font-scale="4"></b-icon>
        <div class="mb-3">Processing...</div>
        <b-progress min="1" max="20" :value="counter" variant="success" height="3px" class="mx-n4 rounded-0"></b-progress>
      </div>
      <div v-else ref="dialog" tabindex="-1" role="dialog" aria-modal="false" aria-labelledby="form-confirm-label" class="text-center p-3">
        <p><strong id="form-confirm-label">Are you sure?</strong></p>
        <div class="d-flex">
          <b-button variant="outline-danger" class="mr-3" @click="onCancel">Cancel</b-button>
          <b-button variant="outline-success" @click="onOK">OK</b-button>
        </div>
      </div>
    </template>
  </b-overlay>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import DeferredPromise, { deferred } from '../DeferredPromise'

export class SubmissionContext {
  public confirmationCallback?: () => void
  public cancelCallback?: () => void
  public successCallback?: () => void
}

@Component
export default class DataSubmitionComponent extends Vue {
  busy = false
  processing = false
  counter = 1
  interval: NodeJS.Timeout|null = null

  private _context = new SubmissionContext()
  private _currentPromise!: DeferredPromise<void>

  private beforeDestroy () {
    this.cleanupInterval()
  }

  private cleanupInterval () {
    if (this.interval) {
      clearInterval(this.interval)
      this.interval = null
    }
  }

  private async onShown () {
    const dialog = this.$refs.dialog as HTMLElement
    if (dialog !== undefined) {
      (this.$refs.dialog as HTMLElement).focus() // Focus the dialog prompt
    }
  }

  private onHidden () {
    // In this case, we return focus to the submit button
    // You may need to alter this based on your application requirements
    // this.$refs.submit.focus()
    if (this.processing) {
      if (this._context.successCallback) {
        this._context.successCallback()
      }
    } else {
      if (this._context.cancelCallback) {
        this._context.cancelCallback()
      }
    }
    this._currentPromise.resolve()
  }

  private onSubmit () {
    this.processing = false
    this.busy = true
  }

  private onCancel () {
    this.busy = false
  }

  private onOK () {
    this.counter = 1
    this.processing = true
    // Simulate an async request
    this.cleanupInterval()
    this.interval = setInterval(() => {
      if (this.counter < 20) {
        this.counter = this.counter + 1
      } else {
        // this.onFinish()
        this.counter = 1
      }
    }, 350)

    if (this._context.confirmationCallback) {
      this._context.confirmationCallback()
    }
  }

  private onFinish () {
    this.cleanupInterval()
    this.$nextTick(() => {
      this.busy = this.processing = false
    })
  }

  public doShow (context?: SubmissionContext): Promise<void> {
    this._currentPromise = deferred<void>()
    this._context = context ?? new SubmissionContext()
    if (this._context.confirmationCallback) {
      this.onSubmit()
    } else {
      this.busy = true
      this.onOK()
    }
    return this._currentPromise
  }

  public doFinish () {
    this.onFinish()
  }
}
</script>
