
export enum WhatToList {
    None = 0,
    Receivers = 1,
    Senders = 2,
    Transformers = 4,
    Filters = 8
  }


export class ResultTypeStankins {
    public type: string;
    public name: string;
    public constructorParam: Map<string, string>;
    public cacheWhatToList: WhatToList;
}
