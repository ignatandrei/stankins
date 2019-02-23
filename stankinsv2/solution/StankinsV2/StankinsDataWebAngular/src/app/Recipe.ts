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
  public searchString(): string {
    return this.content + this.name + this.description;
  }

}
