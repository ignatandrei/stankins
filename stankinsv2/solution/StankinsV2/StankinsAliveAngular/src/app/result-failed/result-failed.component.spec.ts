import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResultFailedComponent } from './result-failed.component';

describe('ResultFailedComponent', () => {
  let component: ResultFailedComponent;
  let fixture: ComponentFixture<ResultFailedComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResultFailedComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResultFailedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
