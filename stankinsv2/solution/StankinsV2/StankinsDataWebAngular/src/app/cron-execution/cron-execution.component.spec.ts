import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CronExecutionComponent } from './cron-execution.component';

describe('CronExecutionComponent', () => {
  let component: CronExecutionComponent;
  let fixture: ComponentFixture<CronExecutionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CronExecutionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CronExecutionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
