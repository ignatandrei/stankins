import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TagDisplayComponent } from './tag-display.component';

describe('TagDisplayComponent', () => {
  let component: TagDisplayComponent;
  let fixture: ComponentFixture<TagDisplayComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TagDisplayComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TagDisplayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
