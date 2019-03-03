import { WhatToList } from './WhatToList';

export class Recipe {

  constructor(r: Recipe= null) {
    if (r == null) {
      return;
    }

    Object.assign(this, r);
  }

  public content: string;
  public name: string;
  public description: string;
  public arguments: string[];
  public whatToList: WhatToList;
  public searchString(): string {
    return (this.content + this.name + this.description).toLocaleLowerCase();
  }
  public CanExecuteDirectly(): boolean {
    return this.arguments == null || this.arguments.length === 0;
  }
  public IsFileRecipe(): boolean {
    return this.whatToList == null;
  }
}
