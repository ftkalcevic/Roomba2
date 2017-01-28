import { Component,Input } from '@angular/core';
import { CurrentStatus } from './CurrentStatus';
import { Mission } from './Mission';
import { MissionDetails } from './MissionDetails';
import { RoombaService } from './roomba.service';
import './rxjs-operators';
import { OnInit } from '@angular/core';

@Component({
    selector: 'my-app',
    styles: [`
  .selected {
    background-color: #CFD8DC !important;
    color: white;
  }
  .missions {
    margin: 0 0 2em 0;
    list-style-type: none;
    padding: 0;
    width: 15em;
  }
  .missions th {
    cursor: pointer;
    position: relative;
    left: 0;
    background-color: #EEE;
    margin: .5em;
    padding: .3em 0;
    height: 1.6em;
    border-radius: 4px;
  }
  .missions li {
    cursor: pointer;
    position: relative;
    left: 0;
    background-color: #EEE;
    margin: .5em;
    padding: .3em 0;
    height: 1.6em;
    border-radius: 4px;
  }
  .missions li.selected:hover {
    background-color: #BBD8DC !important;
    color: white;
  }
  .missions li:hover {
    color: #607D8B;
    background-color: #DDD;
    left: .1em;
  }
  .missions .text {
    position: relative;
    top: -3px;
  }
  .missions .mission {
    display: inline-block;
    font-size: small;
    color: white;
    padding: 0.8em 0.7em 0 0.7em;
    background-color: #607D8B;
    line-height: 1em;
    position: relative;
    left: -1px;
    top: -4px;
    height: 1.8em;
    margin-right: .8em;
    border-radius: 4px 0 0 4px;
  }
`],
    templateUrl: 'app/app.component.html',
    providers: [RoombaService]
})
export class AppComponent implements OnInit {
    roombaStatus: CurrentStatus;
    roombaStatus_LastUpdate :string;
    roombaStatus_RoombaTime :string;
    roombaStatus_NextMission: string;
    nextMissionCountDown: number;
    lastNextMissionCountDown: number;
    roombaMissions: Mission[];
    selectedMission: Mission;
    errorMessage: string;

    constructor(private roombaService: RoombaService) {
        this.roombaStatus = new CurrentStatus();
        this.roombaStatus_LastUpdate = "";
        this.roombaStatus_RoombaTime = "";
        this.roombaStatus_NextMission= "";
        this.roombaMissions = new Array<Mission>();
        this.errorMessage = "";
    }

    onSelect(index: number)
    {
        if ( index >= 0)
            this.selectedMission = this.roombaMissions[index];
        else
            this.selectedMission = { MissionId: -1, StartTime: null, EndTime: null };
    }

    locale(): string {
        //return (navigator.languages && navigator.languages.length) ? navigator.languages[0] : navigator.language;
        return navigator.language;
    }

    updateNextMissionCountdown() {
        if (this.nextMissionCountDown)
        {
            if (this.nextMissionCountDown <= 0) {
                this.roombaStatus_NextMission = 'Now!'
            }
            else {
                this.nextMissionCountDown--;

                let days: number = Math.floor(this.nextMissionCountDown / (24 * 60 * 60));
                let hours: number = Math.floor((this.nextMissionCountDown / (60 * 60)) % 24);
                let mins: number = Math.floor((this.nextMissionCountDown / 60) % 60);
                let secs: number = Math.floor(this.nextMissionCountDown % 60);

                let s: string = "";
                if (days > 0)
                    s += days.toString() + "d ";
                if (!(days == 0 && hours == 0))
                    s += hours.toLocaleString(this.locale(), { minimumIntegerDigits: 2 }) + "h ";
                if (!(days == 0 && hours == 0 && mins == 0))
                    s += mins.toLocaleString(this.locale(), { minimumIntegerDigits: 2 }) + "m ";
                s += secs.toLocaleString(this.locale(), { minimumIntegerDigits: 2 }) + "s";

                this.roombaStatus_NextMission = s;
            }
        }
        setTimeout(() => { this.updateNextMissionCountdown(); }, 1000);
    }

    updateStatus(status: CurrentStatus, error?: string) {
        if (status) {
            this.roombaStatus = status;

            if (!this.lastNextMissionCountDown || this.lastNextMissionCountDown != status.NextMission) {
                this.lastNextMissionCountDown = this.nextMissionCountDown = status.NextMission;
                
            }

            // Date pipes not working on IE
            this.roombaStatus_LastUpdate = new Date(status.LastUpdate).toLocaleDateString(this.locale(), {
                day: 'numeric',
                month: 'short',
                year: 'numeric',
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit'
            });
            this.roombaStatus_RoombaTime = new Date(status.RoombaTime).toLocaleTimeString(this.locale(), {
                hour: '2-digit',
                minute: '2-digit'
            });
        }
        else if (error)
            this.errorMessage = error;

        setTimeout(() => { this.refreshCurrentStatusTimeout(); }, 10000);
    }
    refreshCurrentStatusTimeout()
    {
        this.roombaService.getCurrentStatus()
            .subscribe(status => {
                    this.updateStatus(status);
            },
            error => {
                    this.updateStatus(null,error);
            });
    }

    ngOnInit()
    { 
        this.refreshCurrentStatusTimeout();
        this.updateNextMissionCountdown();

        this.roombaService.getMissions()
            .subscribe(missions => {
                this.roombaMissions = missions;
            },
            error => this.errorMessage = <any>error);
    }
}
