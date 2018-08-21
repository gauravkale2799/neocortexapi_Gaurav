import { Component, OnInit, AfterViewInit } from '@angular/core';
//import * as Plotly from 'plotly.js';
import * as Plotlyjs from 'plotly.js/dist/plotly';
import { neoCortexUtils } from '../neocortexutils';

@Component({
  selector: 'app-ainet',
  templateUrl: './ainet.component.html',
  styleUrls: ['./ainet.component.css']
})
export class AinetComponent implements OnInit, AfterViewInit {

  data: any;
  layout: any;
  xCoordinate = [];
  yCoordinate = [];
  zCoordinate = [];

  constructor() {

  }
  ngOnInit() {
  }
  ngAfterViewInit() {
    this.fillChart();
    this.createChart();
  }
  createChart() {
    let graph = document.getElementById('graph');
    const trace1 = {
      x: this.xCoordinate,
      y: this.yCoordinate,
      z: this.zCoordinate,
      name: 'Data 1',
      mode: 'markers',
      marker: {
        size: 12,

        //color: '#00FFFF',
        symbol: 'circle',
        line: {
          color: '#00FFFF',
          width: 2
        },
        opacity: 0.8
      },
      type: 'scatter3d'

    };
    const trace2 = {
      x: [0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2],
      y: [0, 0, 0, 1, 2, 1, 2, 1, 2, 0, 0, 0, 1, 2, 1, 2, 1, 2, 0, 0, 0, 1, 2, 1, 2, 1, 2],
      z: [0, 1, 2, 0, 0, 1, 1, 2, 2, 0, 1, 2, 0, 0, 1, 1, 2, 2, 0, 1, 2, 0, 0, 1, 1, 2, 2],
      name: 'Data 2',
      mode: 'markers',
      marker: {
        size: 12,

        //color: '#00FFFF',
        symbol: 'circle',
        line: {
          color: 'green',
          width: 2
        },
        opacity: 0.8
      },
      type: 'scatter3d'

    };

    this.data = [trace1, trace2];
    this.layout = {
      dragmode: false,
      autosize: false,
      showlegend: true,
      legend: {
        x: 0.5,
        y: 1
      },
      width: 800,
      height: 800,
      margin: {
        l: 0,
        r: 0,
        b: 0,
        t: 0,
        pad: 4
      }
    };
    const config = {
      displaylogo: false,

    };
    Plotlyjs.newPlot(graph, this.data, this.layout, config);
  }
  fillChart() {
    let model = neoCortexUtils.createModel(2, [100, 3], 6); // createModel (numberOfAreas/DataSeries, [xAxis, zAxis], yAxis)
    let x; let y; let z;
    for (x = 0; x < model.settings.minicolumnDims[0]; x++) {
        for (y = 0; y < model.settings.numLayers; y++) {
            for (z = 0; z < model.settings.minicolumnDims[1]; z++) {
              this.xCoordinate.push(x);
              this.yCoordinate.push(y);
              this.zCoordinate.push(z);
                
            }

        }

    }
/*     for (let a = 0; a < model.settings.numAreas; a++) {
        
    } */
}

}