import { Component, OnInit } from '@angular/core';
import { CronExecutionService, CronExecutionFile } from '../cron-execution.service';

@Component({
  selector: 'app-cron-execution',
  templateUrl: './cron-execution.component.html',
  styleUrls: ['./cron-execution.component.css']
})
export class CronExecutionComponent implements OnInit {

  cronArr: CronExecutionFile[];
  constructor(private cronexec: CronExecutionService) { }

  ngOnInit() {
    this.cronexec.Get().subscribe(it => this.cronArr = it);
  }
  save(cronexec: CronExecutionFile) {
    this.cronexec.Save(cronexec).subscribe(() => window.alert('saved'));
  }
  delete(cronexec: CronExecutionFile) {
    this.cronexec.Delete(cronexec).subscribe(() => this.cronArr.splice(this.cronArr.indexOf(cronexec), 1));
  }
  addNew() {
    const c = CronExecutionFile.NewCron();
    this.cronexec.addNew(c).subscribe(() => this.cronArr.push(c));
  }
}
