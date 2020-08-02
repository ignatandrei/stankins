@{
	var angular="@angular";
}
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { @Name@Component } from './my-test.component';

describe('@Name@Component', () => {
  let component: @Name@Component;
  let fixture: ComponentFixture<@Name@Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ @Name@Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(@Name@Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
