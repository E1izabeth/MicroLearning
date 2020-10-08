// import * as cxml from '@wikipathways/cxml'
// import * as cxsd from '@wikipathways/cxsd'
import fetch, { Headers, Response, RequestInit } from 'node-fetch'
import { register } from 'register-service-worker'
import * as x from '../xml/my-json-types'

function p<T> (arg: Partial<T>): T {
  return arg as T
}

function p2<T, R> (arg: Partial<T>): R {
  return arg as R
}

enum HttpMethod {
  Get,
  Post,
  Delete
}

class HelperImpl {
  private httpMethodNames = new Map<HttpMethod, string>([
    [HttpMethod.Get, 'GET'],
    [HttpMethod.Post, 'POST'],
    [HttpMethod.Delete, 'DELETE']
  ])

  private rootUrl: string

  private cookies = new Map<string, string>()

  public constructor (rootUrl: string) {
    this.rootUrl = rootUrl
  }

  private async request (method: HttpMethod, url: string, arg?: any): Promise<Response> {
    const headers = new Headers()
    headers.set('Accept', 'application/json')

    if (arg !== undefined) {
      headers.set('Content-Type', 'application/json')
    }
    if (this.cookies.size > 0) {
      headers.set('Cookie', Array.from(this.cookies).map(e => e.join('=')).join(';'))
    }

    const response = await fetch(this.rootUrl + url, {
      body: arg ? JSON.stringify(arg) : undefined,
      method: this.httpMethodNames.get(method),
      headers: headers,
      credentials: 'include'
    } as any)

    response.headers.forEach((h, s) => {
      if (h === 'Set-Cookies') {
        const kv = s.split(';')[0].split('=')
        this.cookies.set(kv[0], kv[1])
      }
    })

    if (response.ok) {
      return Promise.resolve(response)
    } else {
      let errMsg: string

      if (response.headers.get('Content-Type')?.startsWith('application/json') === true) {
        const err = await this.parse<x.ErrorInfoType>(response)
        // const text = await response.text()
        // const err = JSON.parse(text) as x.ErrorInfoType
        errMsg = err.Message
      } else {
        errMsg = response.statusText
      }

      console.warn(`Response (status ${response.status}) error message: ${errMsg}`)
      throw new Error(errMsg)
    }
  }

  private async parse<T> (response: Response): Promise<T> {
    const responseText = await response.text()
    if (responseText.length > 0) {
      // const p = new cxml.Parser({ "": "" })
      // const doc = await p.parse(await response.text(), x.document)
      // const doc = x.document
      // return part(doc)
      return Promise.resolve(JSON.parse(responseText) as T)
    } else {
      throw new Error('Expected result but has nothing in response')
    }
  }

  public async delete (url: string): Promise<void> {
    await this.request(HttpMethod.Delete, url)
  }

  public async post (url: string, arg?: any): Promise<void> {
    await this.request(HttpMethod.Post, url, arg)
  }

  /*
  public async post<T> (url: string, arg?: Partial<T>): Promise<void> {
    await this.request(HttpMethod.Post, url, arg)
  }
  */

  public async postAndParse<R> (url: string, arg: any): Promise<R> {
    return await this.parse<R>(await this.request(HttpMethod.Post, url, arg))
  }

  /*
  public async postAndParse<T, R> (url: string, arg?: Partial<T>): Promise<R> {
    return await this.parse<R>(await this.request(HttpMethod.Post, url, arg))
  }
  */

  public async get<R> (url: string): Promise<R> {
    return await this.parse<R>(await this.request(HttpMethod.Get, url))
  }
}

const helper = new HelperImpl('http://' + location.hostname + ':8181/mysvc')

/*
./node_modules/.bin/cxsd file://MicroLearningSvc.expanded.xsd
*/

let _profileFootprint = helper.get<x.ProfileFootprintInfoType>('/profile')

const api = {
  authorized: async function () {
    try {
      await _profileFootprint
      return true
    } catch {
      return false
    }
  },
  currentProfile: async function (): Promise<x.ProfileFootprintInfoType> {
    return await _profileFootprint
  },
  updateCurrentProfile: async function (): Promise<x.ProfileFootprintInfoType> {
    _profileFootprint = helper.get<x.ProfileFootprintInfoType>('/profile')
    return await _profileFootprint
  },
  login: async function (login: string, pwd: string): Promise<void> {
    await helper.post('/profile?action=login', p<x.LoginSpecType>({ Login: login, Password: pwd }))
    await this.updateCurrentProfile()
  },
  logout: async function (): Promise<void> {
    await helper.post('/profile?action=logout')
    try { await this.updateCurrentProfile() } catch { }
  },
  register: async function (login: string, pwd: string, email: string): Promise<void> {
    return helper.post('/profile?action=register', p<x.RegisterSpecType>({ Login: login, Password: pwd, Email: email }))
  },
  setEmail: async function (oldEmail: string, newEmail: string, password: string): Promise<void> {
    await helper.post('/profile?action=set-email', p<x.ChangeEmailSpecType>({ OldEmail: oldEmail, NewEmail: newEmail, Password: password }))
    await this.updateCurrentProfile()
  },
  setPassword: async function (oldEmail: string, newPassword: string): Promise<void> {
    return helper.post('/profile?action=set-password', p<x.ChangePasswordSpecType>({ Email: oldEmail, NewPassword: newPassword }))
  },
  requestActivation: async function (email: string): Promise<void> {
    return helper.post('/profile?action=activate', p<x.RequestActivationSpecType>({ Email: email }))
  },
  requestAccessRestore: async function (login: string, email: string): Promise<void> {
    return helper.post('/profile?action=restore', p<x.ResetPasswordSpecType>({ Email: email, Login: login }))
  },
  performActivateAction: async function (key: string): Promise<void> {
    await helper.get('/profile?action=activate&key=' + key)
    await this.updateCurrentProfile()
  },
  performRestoreAccessAction: async function (key: string): Promise<void> {
    await helper.get('/profile?action=restore&key=' + key)
    await this.updateCurrentProfile()
  },
  /*
  [OperationContract, WebInvoke(UriTemplate = "/profile?action=delete", Method = "POST")]
  void DeleteProfile();
  */
  suggestResource: async function (title: string, url: string, tags: string[]): Promise<x.CreateResourceSpecType> {
    const spec = p<x.CreateResourceSpecType>({
      ResourceUrl: url,
      ResourceTitle: title,
      AssociationTags: tags.map(v => p<x.AssociationTagInfoType>({ Word: v }))
    })
    return helper.postAndParse<x.CreateResourceSpecType>('/resources?action=suggest', spec)
  },
  createResource: async function (title: string, url: string, tags: string[]): Promise<x.ResourceInfoType> {
    const spec = p<x.CreateResourceSpecType>({
      ResourceUrl: url,
      ResourceTitle: title,
      AssociationTags: tags.map(v => p2<x.AssociationTagInfoType, x.AssociationTagInfoType>({ Word: v }))
    })
    return helper.postAndParse<x.ResourceInfoType>('/resources?action=create', spec)
  },
  getResources: async function (): Promise<x.ResourcesListType> {
    return helper.get<x.ResourcesListType>('/resources/')
  },
  getResourcesRange: async function (skip: number, take: number): Promise<x.ResourcesListType> {
    return helper.get<x.ResourcesListType>(`/resources/?from=${skip}&count=${take}`)
  },
  filterResourcesRange: async function (filter: { topicId?: number; tags?: string[] }, skip: number, take: number): Promise<x.ResourcesListType> {
    let spec: x.ResourceFilterSpecType

    if (filter.topicId !== undefined) {
      spec = p<x.ResourceFilterSpecType>({
        Item: p<x.ResourceFilterByTopicSpec>({
          _type: 'ByTopic',
          TopicId: filter.topicId
        })
      })
    } else if (filter.tags !== undefined) {
      spec = p<x.ResourceFilterSpecType>({
        Item: p<x.ResourceFilterByKeywordsSpec>({
          _type: 'ByKeywords',
          AssociationTags: filter.tags.map(v => p2<x.AssociationTagInfoType, x.AssociationTagInfoType>({ Word: v }))
        })
      })
    } else {
      throw new Error('filter spec expected')
    }
    return helper.postAndParse<x.ResourcesListType>(`/resources/?from=${skip}&count=${take}`, spec)
  },
  getResourceById: async function (resId: number): Promise<x.ResourceInfoType> {
    return helper.get<x.ResourceInfoType>(`/resources/${resId}`)
  },
  markResourceValidated: async function (resId: number): Promise<void> {
    return helper.post(`/resources/${resId}?action=validate`, '')
  },
  createTopic: async function (name: string, tags: string[]): Promise<x.TopicInfoType> {
    const spec = p<x.CreateTopicSpecType>({
      TopicName: name,
      AssociationTags: tags.map(v => p2<x.AssociationTagInfoType, x.AssociationTagInfoType>({ Word: v }))
    })
    return helper.postAndParse<x.TopicInfoType>('/topics?action=create', spec)
  },
  getTopics: async function (): Promise<x.TopicsListType> {
    return helper.get<x.TopicsListType>('/topics/')
  },
  getTopicsRange: async function (skip: number, take: number): Promise<x.TopicsListType> {
    return helper.get<x.TopicsListType>(`/topics/?from=${skip}&count=${take}`)
  },
  getTopicById: async function (topicId: number): Promise<x.TopicInfoType> {
    return helper.get<x.TopicInfoType>(`/topics/${topicId}`)
  },
  deleteTopic: async function (topicId: number): Promise<void> {
    return helper.delete(`/topics/${topicId}`)
  },
  resetTopicLearningProgress: async function (topicId: number): Promise<void> {
    return helper.post(`/topics/${topicId}?action=reset`)
  },
  activateTopicLearningProgress: async function (topicId: number, dueSeconds: number, intervalSeconds: number): Promise<void> {
    const spec = p<x.ActivateTopicSpecType>({
      DueSeconds: dueSeconds,
      IntervalSeconds: intervalSeconds
    })
    return helper.post(`/topics/${topicId}?action=activate`, spec)
  },
  deactivateTopicLearningProgress: async function (topicId: number): Promise<void> {
    return helper.post(`/topics/${topicId}?action=deactivate`)
  },
  getContentPartsByTopic: async function (topicId: number, skip: number, take: number): Promise<x.ContentPartsListType> {
    return helper.get<x.ContentPartsListType>(`/topics/${topicId}/parts?from=${skip}&count=${take}`)
  },
  // [OperationContract, WebGet(UriTemplate = "/resources/{id}/parts?from={skip}&count={take}")]
  // ContentPartsListType GetContentPartsByResource(string resourceIdStr, string skip, string take);
  markContentPartLearned: async function (topicId: number, partId: number): Promise<void> {
    return helper.post(`/topics/${topicId}/parts/${partId}?action=mark`)
  },
  unmarkContentPartLearned: async function (topicId: number, partId: number): Promise<void> {
    return helper.post(`/topics/${topicId}/parts/${partId}?action=unmark`)
  }
}

export default api
