import { Component, Input, OnChanges, SimpleChange, HostListener, ElementRef } from '@angular/core';
import { Mission } from './Mission';
import { MissionDetails } from './MissionDetails';
import { LiveMissionDetails } from './LiveMissionDetails';
import { RoombaService } from './roomba.service';
import './rxjs-operators';


@Component({
    selector: 'roomba-map',
    templateUrl: 'app/roomba-map.component.html',
    providers: [RoombaService]
})
export class RoombaMapComponent implements OnChanges {
    @Input() missionNumber: number;
    @Input() dScale: number;
    @Input() dRotate: number;
    @Input() dOffsetX: number;
    @Input() dOffsetY: number;
    @Input() dLiveRefreshPeriod: number = 5000;
    @Input() bAccumulatePath: boolean;
    @Input() bThickLine: boolean;
    numberOfPoints: number;
    details: MissionDetails;
    errorMessage: string;
    pointsToShow: number;
    xpoints: number[];
    ypoints: number[];
    thetas: number[];
    lastTick: number;
    lastAnimationEndFrame: number;
    animationStart: number;
    animationEnd: number;
    animationStartFrame: number;
    animationEndFrame: number;
    roombaImg: HTMLCanvasElement;
    roombaCanvas: any;

    //@HostListener('window:resize', ['$event'])
    //onResize(event:any) {
    //    this.RedrawMission();
    //}
    constructor(private elref: ElementRef,
        private roombaService: RoombaService) {
        this.details = new MissionDetails()
        this.missionNumber = -1;
        this.bAccumulatePath = true;
        this.bThickLine = true;
        this.numberOfPoints = 0;
        this.pointsToShow = 0;
        this.roombaCanvas = <HTMLCanvasElement>document.createElement('canvas');
        this.roombaCanvas.width = 500;
        this.roombaCanvas.height = 500;
        let ctx: CanvasRenderingContext2D = this.roombaCanvas.getContext("2d");


    }

    Scale( min1: number, max1: number, min2: number, max2: number): number[]
    {
        let scale: number;
        let offset: number;

        if (max1 == min1)
            scale = 1;
        else
            scale = (max2 - min2) / (max1 - min1);
        offset = min1;
        return [scale, offset];
    }

    draw(from: number, to: number, bDrawHome: boolean, bDrawRoomba: boolean, bClear: boolean, sCanvas?: string) {
        let canvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById(sCanvas ? sCanvas : "canvasMap");
        let ctx: CanvasRenderingContext2D = canvas.getContext("2d");

        if ( bClear ) ctx.clearRect(0, 0, canvas.width, canvas.height);

        if (this.xpoints.length === 0) {
            return;
        }
        ctx.save();

        ctx.translate(this.dOffsetX, this.dOffsetY);
        ctx.scale(this.dScale, -this.dScale);
        ctx.rotate(this.dRotate * Math.PI / 180);

        // Home
        if (bDrawHome) {
            ctx.fillStyle = "lime";
            ctx.beginPath();
            ctx.moveTo(-15, 15);
            ctx.lineTo(15, 15);
            ctx.lineTo(15, -12);
            ctx.lineTo(22, -12);
            ctx.lineTo(0, -27);
            ctx.lineTo(-22, -12);
            ctx.lineTo(-15, -12);
            ctx.lineTo(-15, 15);
            ctx.fill();
        }

        if (this.bThickLine) {
            ctx.lineCap = "round";
            ctx.lineJoin = "round";
            ctx.lineWidth = 20;
        }
        else 
            ctx.lineWidth = 1;

        if (this.bAccumulatePath) {
            ctx.lineCap = "butt";
            ctx.lineJoin = "round";
            ctx.strokeStyle = "RGBA(0,0,255,0.2)";
        } else {
            ctx.strokeStyle = 'blue';
        }


        let x: number = this.xpoints[from];
        let y: number = this.ypoints[from];

        if (this.bAccumulatePath) {
            let bFirst: boolean = true;
            for (let i = from+1; i <= to; i++) {
                ctx.beginPath();
                ctx.moveTo(x, y);
                x = this.xpoints[i];
                y = this.ypoints[i];
                ctx.lineTo(x, y);
                ctx.stroke();
            }
        }
        else {
            ctx.beginPath();
            ctx.moveTo(x, y);
            for (let i = from+1; i <= to; i++) {
                x = this.xpoints[i];
                y = this.ypoints[i];
                ctx.lineTo(x, y);
            }
            ctx.stroke();
        }

        // Roomba
        if (bDrawRoomba) {
            let theta: number = this.thetas[to];
            this.DrawRoomba(ctx, x, y, theta);
        }

        ctx.restore();
    }

    DrawRoomba(ctx: CanvasRenderingContext2D, x: number, y: number, theta: number) {
        let rsize: number = 40;

        //console.log("x:" + x.toString() + ", y:" + y.toString() + ", theta:" + theta.toString());

        ctx.save();
        ctx.translate(x, y);
        ctx.rotate((theta + 90) * Math.PI / 180);
        let image: HTMLImageElement = <HTMLImageElement>document.getElementById('imgRoomba');
        ctx.drawImage(image, -rsize / 2, -rsize / 2, rsize, rsize);
        ctx.restore();
    }

    RedrawMission() {
        this.draw(0,this.pointsToShow-1, true,true,true);
    }

    RenderAnimationFrame(clock: number) {
        let endFrame: number = (this.animationEndFrame - this.animationStartFrame) * (clock - this.animationStart) / this.dLiveRefreshPeriod + this.animationStartFrame;
        if (Math.floor(endFrame) != this.lastAnimationEndFrame) {

            this.draw(0, Math.floor(endFrame), true, false, true);
            this.lastAnimationEndFrame = Math.floor(endFrame);
        }

       // console.log("from:" + this.animationStartFrame.toString() + ", to:" + endFrame.toString());
        if (endFrame > this.xpoints.length-1)
            endFrame = this.xpoints.length-1;

        {

            let canvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("canvasAnimation");
            let ctx: CanvasRenderingContext2D = canvas.getContext("2d");
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            ctx.save();

            ctx.translate(this.dOffsetX, this.dOffsetY);
            ctx.scale(this.dScale, -this.dScale);
            ctx.rotate(this.dRotate * Math.PI / 180);

            if (this.bThickLine) {
                ctx.lineCap = "round";
                ctx.lineJoin = "round";
                ctx.lineWidth = 20;
            }
            else
                ctx.lineWidth = 1;

            if (this.bAccumulatePath) {
                ctx.lineCap = "butt";
                ctx.lineJoin = "round";
                ctx.strokeStyle = "RGBA(0,0,255,0.2)";
            } else {
                ctx.strokeStyle = 'blue';
            }


            let frame: number = Math.floor(endFrame);
            let percent: number = endFrame - frame;
            let x: number = this.xpoints[frame];
            let y: number = this.ypoints[frame];
            let theta: number = this.thetas[frame];

            let frame2: number = frame == this.xpoints.length-1 ? frame : frame+1;
            let x2: number = this.xpoints[frame2];
            let y2: number = this.ypoints[frame2];
            let theta2: number = this.thetas[frame2];

            let dtheta2: number = theta2 - theta;
            dtheta2 += (dtheta2 > 180) ? -360 : (dtheta2 < -180) ? 360 : 0;

            let x1: number = x + (x2 - x)*percent;
            let y1: number = y + (y2 - y)*percent;
            let theta1: number = theta + (dtheta2)*percent;
            //console.log("theta:" + theta.toString() + ", theta2:" + theta2.toString() + ", dtheta2:" + dtheta2.toString() + ", theta1:" + theta1.toString());

            if (isNaN(theta1) )
                theta1 = 0;

            ctx.beginPath();
            ctx.moveTo(x, y);
            ctx.lineTo(x1, y1);
            ctx.stroke();
                
            // Roomba
            this.DrawRoomba(ctx, x1, y1, theta1);

            ctx.restore();
        }
        window.requestAnimationFrame(clock => { this.RenderAnimationFrame(clock); });
    }

    AnimateMission(from:number, to:number) {
        this.animationStart = performance.now();
        this.animationEnd = this.animationStart + 5 * 1000;
        this.animationStartFrame = from-1;
        this.animationEndFrame = to;
        this.lastAnimationEndFrame = this.animationStartFrame - 1;
        //this.draw(0, from, true, false, true);
        window.requestAnimationFrame(clock => { this.RenderAnimationFrame(clock); });
    }

    UpdateLiveMissionDetails(details: LiveMissionDetails) {
        if (this.lastTick == 0) {
            if (details.LastTick > 0) {
                this.InitDetails(details.x, details.y, details.theta);
                this.lastTick = details.LastTick;
                this.RedrawMission();
            }
        }
        else if (this.lastTick != details.LastTick) {
            let from: number = this.xpoints.length;
            this.AppendDetails(details.x, details.y, details.theta);
            this.lastTick = details.LastTick;
            this.AnimateMission(from, this.xpoints.length-1);
            //this.RedrawMission();
        }
        setTimeout(() => {
            this.roombaService.getLiveMissionDetails(this.lastTick)
                .subscribe(details => {
                    this.UpdateLiveMissionDetails(details);
                },
                error => this.errorMessage = <any>error);
        }, this.dLiveRefreshPeriod);
    }

    InitDetails(x: number[], y: number[], theta: number[] ) {
        this.numberOfPoints = x.length;
        this.pointsToShow = this.numberOfPoints;
        //document.getElementById('points').nodeValue = this.pointsToShow.toString();

        this.xpoints = x;
        this.ypoints = y;
        this.thetas = theta;
    }
    AppendDetails(x: number[], y: number[], theta: number[]) {

        this.numberOfPoints += x.length;
        this.pointsToShow += x.length;
        //document.getElementById('points').nodeValue = this.pointsToShow.toString();

        this.xpoints = this.xpoints.concat( x );
        this.ypoints = this.ypoints.concat( y );
        this.thetas = this.thetas.concat(theta);
    }
    NewMissionDetails(details: MissionDetails) {
        this.details = details;

        this.InitDetails( details.x, details.y, details.theta )

        this.RedrawMission();
    }

    ngOnChanges(changes: { [propName: string]: SimpleChange }) {
        if (changes["missionNumber"]) {

            this.missionNumber = changes["missionNumber"].currentValue;
            if (this.missionNumber) {

                if (this.missionNumber == -1) {
                    this.lastTick = 0;
                    this.roombaService.getLiveMissionDetails(this.lastTick)
                        .subscribe(details => {
                            this.UpdateLiveMissionDetails(details);
                        },
                        error => this.errorMessage = <any>error);
                }
                else {
                    this.roombaService.getMissionDetails(this.missionNumber)
                        .subscribe(details => {
                                this.NewMissionDetails(details);
                        },
                        error => this.errorMessage = <any>error);
                }
            }
        }
    }

    onLineThicknessChanged(bThick: boolean) {
        this.bThickLine = bThick;
        this.RedrawMission();
    }

    onAccumulateChanged(bAccum: boolean) {
        this.bAccumulatePath = bAccum;
        this.RedrawMission();
    }

    onRangeChanged(points: number) {
        this.pointsToShow = points;
        this.RedrawMission();
    }

}