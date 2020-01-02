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
//class App extends React.Component {
//    constructor(props: string) {
//        super(props);
//        this.state = {
//            text: 'Hello Work!'
//        }
//    }
//    render() {
//        return (
//            <div>
//                <p>Hello Work!</p>
//            </div>
//        )
//    }
//}
var Hello = /** @class */ (function (_super) {
    __extends(Hello, _super);
    function Hello() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    Hello.prototype.render = function () {
        var compiler = this.props.compiler;
        return (React.createElement("div", null,
            React.createElement("div", null, compiler),
            React.createElement("div", null, this.props.framework)));
    };
    Hello.defaultProps = {
        compiler: 'Hello Work!'
    };
    return Hello;
}(React.Component));
var hwElement = React.createElement(Hello, { framework: "React" });
ReactDOM.render(hwElement, document.getElementById('app01'));
// Factoryを作ってから描画する方法
var hwFactory = React.createFactory(Hello);
ReactDOM.render(hwFactory({ framework: 'ananan' }), document.getElementById('app02'));
// 動作確認
alert("正常です");
//# sourceMappingURL=doodle.js.map