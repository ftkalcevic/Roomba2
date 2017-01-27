import { Component, Input, OnChanges, SimpleChange, HostListener, ElementRef } from '@angular/core';
import { Mission } from './Mission';
import { MissionDetails } from './MissionDetails';
import { RoombaService } from './roomba.service';
import './rxjs-operators';


@Component({
    selector: 'roomba-map',
    templateUrl: 'app/roomba-map.component.html',
    providers: [RoombaService]
})
export class RoombaMapComponent implements OnChanges {
    @Input() missionId: number;
    @Input() dScale: number;
    @Input() dRotate: number;
    @Input() dOffsetX: number;
    @Input() dOffsetY: number;
    @Input() bAccumulatePath: boolean;
    @Input() bThickLine: boolean;
    numberOfPoints: number;
    details: MissionDetails;
    errorMessage: string;
    pointsToShow: number;
    xpoints: number[];
    ypoints: number[];

    //@HostListener('window:resize', ['$event'])
    //onResize(event:any) {
    //    this.RedrawMission();
    //}
    constructor(private elref: ElementRef,
        private roombaService: RoombaService) {
        this.details = new MissionDetails()
        this.missionId = -1;
        this.bAccumulatePath = true;
        this.bThickLine = true;
        this.numberOfPoints = 0;
        this.pointsToShow = 0;
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

    RedrawMission() {
        let canvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("canvasMap");
        let ctx: CanvasRenderingContext2D = canvas.getContext("2d");

        ctx.clearRect(0, 0, canvas.width, canvas.height);

        if (this.xpoints.length === 0) {
            return;
        }
        ctx.save();

        ctx.rotate(this.dRotate * Math.PI / 180);
        ctx.translate(this.dOffsetX, this.dOffsetY);
        ctx.scale(this.dScale, this.dScale);

        // Home
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


        let x: number = this.xpoints[0];
        let y: number = this.ypoints[0];

        if (this.bAccumulatePath) {
            let bFirst: boolean = true;
            for (let i = 1; i < this.pointsToShow; i++) {
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
            for (let i = 1; i < this.pointsToShow; i++) {
                x = this.xpoints[i];
                y = this.ypoints[i];
                ctx.lineTo(x, y);
            }
            ctx.stroke();
        }

        // Roomba
        ctx.fillStyle = "red";
        ctx.beginPath();
        ctx.arc(x, y, 10, 0, 2 * Math.PI);  // Roomba seems to be about 20 units wide
        ctx.fill();

        ctx.restore();
    }

    NewMissionDetails(details: MissionDetails) {
        this.details = details;

        this.numberOfPoints = this.details.x.length;
        this.pointsToShow = this.numberOfPoints;

        document.getElementById('points').nodeValue = this.pointsToShow.toString();

        this.xpoints = new Array(this.details.x.length);
        this.ypoints = new Array(this.details.x.length);
            
        let xMin: number = this.details.x[0];
        let xMax: number = this.details.x[0];
        let yMin: number = this.details.y[0];
        let yMax: number = this.details.y[0];
        for (let x of this.details.x)
            if (x < xMin)
                xMin = x;
            else if (x > xMax)
                xMax = x;

        for (let y of this.details.y)
            if (y < yMin)
                yMin = y;
            else if (y > yMax)
                yMax = y;

        let xScale: number, xOffset: number;
        let yScale: number, yOffset: number;

        let canvas: HTMLCanvasElement = <HTMLCanvasElement>document.getElementById("canvasMap");
        let ctx: CanvasRenderingContext2D = canvas.getContext("2d");

        const border: number = 5;

        [xScale, xOffset] = this.Scale(xMin, xMax, border, canvas.width-2*border);
        [yScale, yOffset] = this.Scale(yMin, yMax, border, canvas.height - 2 * border);

        if (xScale > yScale) {
            xScale = yScale;
            xOffset -= (((canvas.width - 2 * border) - (xMax - xMin) * xScale) / 2) / xScale;
        }
        else
            yScale = xScale;

        let bFirst: boolean = true;
        for (let i in this.details.x)
        {
            this.xpoints[i] = (this.details.x[i] - xOffset) * xScale;
            this.ypoints[i] = canvas.height - (this.details.y[i] - yOffset) * yScale;

            this.xpoints[i] = this.details.x[i];
            this.ypoints[i] = this.details.y[i];
        }

        this.RedrawMission();
    }

    ngOnChanges(changes: { [propName: string]: SimpleChange }) {
        if (changes["missionId"]) {

            this.missionId = changes["missionId"].currentValue;
            if (this.missionId)
                this.roombaService.getMissionDetails(this.missionId)
                    .subscribe(details => {
                            this.NewMissionDetails(details);
                    },
                    error => this.errorMessage = <any>error);
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