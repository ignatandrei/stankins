import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayNetCoreComponent } from './display-net-core.component';

describe('DisplayNetCoreComponent', () => {
  let component: DisplayNetCoreComponent;
  let fixture: ComponentFixture<DisplayNetCoreComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DisplayNetCoreComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayNetCoreComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
