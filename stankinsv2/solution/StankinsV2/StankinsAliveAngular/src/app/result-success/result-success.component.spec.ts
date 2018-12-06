import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResultSuccessComponent } from './result-success.component';

describe('ResultSuccessComponent', () => {
  let component: ResultSuccessComponent;
  let fixture: ComponentFixture<ResultSuccessComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResultSuccessComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResultSuccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
