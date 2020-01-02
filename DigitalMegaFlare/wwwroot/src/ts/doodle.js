// 何で別でReactをCDNして、こっちをコメントアウトすればいいのかわからない。一切インポートできないってこと？
//import * as React from "react";
//import * as ReactDOM from "react-dom";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var App = /** @class */ (function (_super) {
    __extends(App, _super);
    function App(props) {
        var _this = _super.call(this, props) || this;
        _this.state = {
            text: 'Hello Work!'
        };
        return _this;
    }
    App.prototype.render = function () {
        return (React.createElement("div", null,
            React.createElement("p", null, "Hello Work!")));
    };
    return App;
}(React.Component));
// 何でエラーが出るのかわからない
// TSでReact書くための基礎を調べること
//<p>{this.state.text} </p>
//ReactDOM.render(<App />, 'aaaa');
ReactDOM.render(React.createElement(App, null), document.getElementById('doodle'));
var HelloWork = /** @class */ (function (_super) {
    __extends(HelloWork, _super);
    function HelloWork(props) {
        var _this = _super.call(this, props) || this;
        _this.state = {
            text: 'Hello Work!'
        };
        return _this;
    }
    HelloWork.prototype.render = function () {
        //return <div>Hello Work {this.props.name} </div>;
        return React.createElement("div", null, "Hello Work !! ");
    };
    return HelloWork;
}(React.Component));
// 動的に変数を入れることができないのは何で？
//const hwElement = <HelloWork name="01"/>;
//ReactDOM.render(hwElement, document.getElementById('app01'));
var hwFactory = React.createFactory(HelloWork);
ReactDOM.render(hwFactory({ name: '02' }), document.getElementById('app02'));
// 動作確認
alert("正常です");
//# sourceMappingURL=doodle.js.map